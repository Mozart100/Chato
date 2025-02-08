using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chato.Server.Hubs;

public record HubDownloadInfo(int Amount);
public record GetAllChatReponse(string ChatName);

public interface IChatHub
{

    Task SendTextToChat(MessageInfo messageInfo);
    //Task SendTextToChat(string chat, string fromUser, string message);
    Task SendText(string fromUser, string message);
    Task SendNotificationn(string chatname, string message);
}

[Authorize]
public class ChattoHub : Hub<IChatHub>
{
    public const string HubMapUrl = "/rtxrazgavor";

    public const string User_Connected_Message = $"You are connected to {IChatService.Lobi} chat";

    private readonly IUserService _userService;
    private readonly IChatService _roomService;
    private readonly IAssignmentService _assignmentService;

    public ChattoHub(
        IUserService userService,
        IChatService roomService,
        IAssignmentService assignmentService,
        IHttpContextAccessor httpContextAccessor)
    {
        this._userService = userService;
        this._roomService = roomService;
        this._assignmentService = assignmentService;
    }


    public override async Task OnConnectedAsync()
    {
        var ptr = Encoding.UTF8.GetBytes("Your are connected");

        var connectionId = Context.ConnectionId;
        var user = Context.User;

        await _userService.AssignConnectionId(user.Identity.Name, connectionId);
        await JoinLobiChatInternal();



        await ReplyMessage("server", User_Connected_Message);
        await NotifyUserJoined(user.Identity.Name, IChatService.Lobi);

        await base.OnConnectedAsync();

        //await JoinLobiChatInternal();
    }

    public Task ReplyMessage(string fromUser, string message)
    {
        return Clients.Caller.SendText(fromUser, message);
    }

    public async Task SendMessageToOthersInChat(MessageInfo messageInfo)
    {

        if (messageInfo.ChatName.IsNullOrEmpty())
        {
            throw new ArgumentNullException("Chat cannot be empty");
        }

        //if (messageInfo.ChatName.Equals("anatoliy__nathan"))
        //{

        //}


        var isExists = await _roomService.IsChatExists(messageInfo.ChatName);
        if (isExists)
        {
            var senderInfo = await _roomService.SendMessageAsync(messageInfo.ChatName, messageInfo.FromUser, messageInfo.TextMessage, messageInfo.Image, messageInfo.SenderInfoType);
            messageInfo = new MessageInfo(senderInfo.SenderInfoType, messageInfo.ChatName, messageInfo.FromUser, senderInfo.TextMessage, senderInfo.Image, senderInfo.TimeStemp);
            //messageInfo = new MessageInfo(senderInfo.SenderInfoType, messageInfo.ChatName, messageInfo.FromUser, messageInfo.TextMessage, messageInfo.Image, senderInfo.TimeStemp);

            if (messageInfo.SenderInfoType == SenderInfoType.Image)
            {
                await Clients.Group(messageInfo.ChatName).SendTextToChat(messageInfo);
            }
            else
            {
                await Clients.OthersInGroup(messageInfo.ChatName).SendTextToChat(messageInfo);
            }
        }
        else
        {
            var senderInfo = await JoinOrCreateChatInternal(Context.ConnectionId, messageInfo.FromUser, messageInfo.ChatName,ChatType.Public);

            var toUser = IChatService.GetToUser(messageInfo.ChatName);
            var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(toUser);
            if (user is null)
            {
                throw new ArgumentNullException($"{toUser} doesnt exists.");
            }

            await JoinOrCreateChatInternal(user.ConnectionId, toUser, messageInfo.ChatName, ChatType.Public);
            messageInfo = new MessageInfo(senderInfo.SenderInfoType, messageInfo.ChatName, messageInfo.FromUser, messageInfo.TextMessage, messageInfo.Image, senderInfo.TimeStemp);
            await Clients.OthersInGroup(messageInfo.ChatName).SendTextToChat(messageInfo);
        }
    }

    public async Task JoinOrCreateChat(string chatName, ChatType chatType)
    {
        var userName = Context.User.Identity.Name;
        await JoinOrCreateChatInternal(Context.ConnectionId, userName, chatName, chatType);
        await NotifyUserJoined(userName, chatName);
    }

    public async Task NotifyUserJoined(string user, string chatName)
    {
        await Clients.All.SendNotificationn(chatName, user);
    }

    public async IAsyncEnumerable<MessageInfo> DownloadHistory(string chatName)
    {
        //if (chatName.Equals("anatoliy__nathan"))
        //{

        //}

        var isExists = await _roomService.IsChatExists(chatName);
        if (isExists)
        {
            var list = await _roomService.GetGroupHistoryAsync(chatName);

            foreach (var senderInfo in list)
            {
                string message = null;
                if (senderInfo.SenderInfoType != SenderInfoType.Image)
                {
                    message = senderInfo.TextMessage;
                }

                yield return new MessageInfo(senderInfo.SenderInfoType, chatName, senderInfo.FromUser, message, senderInfo.Image, senderInfo.TimeStemp);
                await Task.Delay(20);
            }
        }
    }

    public async IAsyncEnumerable<GetAllChatReponse> GetAllPublicChats()
    {
        //if (chatName.Equals("anatoliy__nathan"))
        //{

        //}

        var chats = await _roomService.GetChatsAsync(x=>x.ChatType == ChatType.Public);
        foreach (var chat in chats)
        {
            yield return new GetAllChatReponse(chat.ChatName);
        }

        //var isExists = await _roomService.IsChatExists(chatName);
        //if (isExists)
        //{
        //    var list = await _roomService.GetGroupHistoryAsync(chatName);

        //    foreach (var senderInfo in list)
        //    {
        //        string message = null;
        //        if (senderInfo.SenderInfoType != SenderInfoType.Image)
        //        {
        //            message = senderInfo.TextMessage;
        //        }

        //        yield return new MessageInfo(senderInfo.SenderInfoType, chatName, senderInfo.FromUser, message, senderInfo.Image, senderInfo.TimeStemp);
        //        await Task.Delay(20);
        //    }
        //}
    }



    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await _assignmentService.LeaveGroupByUserNameOrIdAsync(Context.User.Identity.Name);
    }

    public async Task UserDisconnectAsync()
    {
        await OnDisconnectedAsync(null);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        await _assignmentService.LeaveGroupByUserNameOrIdAsync(Context.User.Identity.Name);
    }


    public async Task RemoveChatHistory(string groupName)
    {
        await _roomService.RemoveHistoryByRoomNameAsync(groupName);

    }

    private async Task JoinLobiChatInternal()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, IChatService.Lobi);
        await _assignmentService.JoinOrCreateRoom(Context.User.Identity.Name, IChatService.Lobi, ChatType.Public);
    }
    private async Task<SenderInfo> JoinOrCreateChatInternal(string connectionId, string userName, string roomName, ChatType chatType)
    {
        await Groups.AddToGroupAsync(connectionId, roomName);
        return await _assignmentService.JoinOrCreateRoom(userName, roomName, chatType);
    }


}
