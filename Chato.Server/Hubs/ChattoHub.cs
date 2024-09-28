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
        IAssignmentService assignmentService)
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
        await base.OnConnectedAsync();
    }

    public Task ReplyMessage(string fromUser, string message)
    {
        return Clients.Caller.SendText(fromUser, message);
    }

    public async Task SendMessageToOthersInChat(MessageInfo messageInfo)
    {
        //var ptr = Encoding.UTF8.GetBytes(message);
        //var (chatName, fromUser, message, image) = messageInfo;

        if (messageInfo.ChatName.IsNullOrEmpty())
        {
            throw new ArgumentNullException("Chat cannot be empty");
        }

        var isExists = await _roomService.IsChatExists(messageInfo.ChatName);
        if (isExists)
        {
            await _roomService.SendMessageAsync(messageInfo.ChatName, messageInfo.FromUser, messageInfo.TextMessage, messageInfo.Image);
            await Clients.OthersInGroup(messageInfo.ChatName).SendTextToChat(messageInfo);
        }
        else
        {
            await JoinOrCreateChatInternal(Context.ConnectionId, messageInfo.FromUser, messageInfo.ChatName);

            var toUser = IChatService.GetToUser(messageInfo.ChatName);
            var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(toUser);
            if (user is null)
            {
                throw new ArgumentNullException($"{toUser} doesnt exists.");
            }

            await JoinOrCreateChatInternal(user.ConnectionId, toUser, messageInfo.ChatName);
            await Clients.OthersInGroup(messageInfo.ChatName).SendTextToChat(messageInfo);
        }
    }

    public async Task JoinOrCreateChat(string chatName)
    {
        var userName = Context.User.Identity.Name;
        await JoinOrCreateChatInternal(Context.ConnectionId, userName, chatName);
        await NotifyUserJoined(userName, chatName);
    }

    public async Task NotifyUserJoined(string user, string chatName)
    {
        await Clients.All.SendNotificationn(chatName, user);
    }

    public async IAsyncEnumerable<MessageInfo> DownloadHistory(string chatName)
    {
        var isExists = await _roomService.IsChatExists(chatName);
        if (isExists)
        {
            var list = await _roomService.GetGroupHistoryAsync(chatName);

            foreach (var senderInfo in list)
            {
                yield return new MessageInfo(chatName, senderInfo.FromUser, senderInfo.TextMessage, senderInfo.Image);
                await Task.Delay(20);
            }
        }
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await _assignmentService.RemoveUserByUserNameOrIdAsync(Context.User.Identity.Name);
    }

    public async Task UserDisconnectAsync()
    {
        await OnDisconnectedAsync(null);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        await _assignmentService.RemoveUserByUserNameOrIdAsync(Context.User.Identity.Name);
    }


    public async Task RemoveChatHistory(string groupName)
    {
        await _roomService.RemoveHistoryByRoomNameAsync(groupName);

    }

    private async Task JoinLobiChatInternal()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, IChatService.Lobi);
        await _assignmentService.JoinOrCreateRoom(Context.User.Identity.Name, IChatService.Lobi);
    }
    private async Task JoinOrCreateChatInternal(string connectionId, string userName, string roomName)
    {
        await Groups.AddToGroupAsync(connectionId, roomName);
        await _assignmentService.JoinOrCreateRoom(userName, roomName);
    }


}
