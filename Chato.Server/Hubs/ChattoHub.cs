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

    Task SendTextToChat(string chat, string fromUser, string message);
    Task SendText(string fromUser, string message);
    //Task SelfReplay(string message);
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

    public async Task SendMessageToOthersInGroup(string chatName, string fromUser, string message)
    {
        //var ptr = Encoding.UTF8.GetBytes(message);
        if (chatName.IsNullOrEmpty())
        {
            throw new ArgumentNullException("Chat cannot be empty");
        }

        var isExists = await _roomService.IsChatExists(chatName);
        if (isExists)
        {
            await _roomService.SendMessageAsync(chatName, fromUser, message);
            await Clients.OthersInGroup(chatName).SendTextToChat(chatName, fromUser, message);
        }
        else
        {
            await JoinOrCreateChatInternal(Context.ConnectionId, fromUser, chatName);

            var toUser = IChatService.GetToUser(chatName);
            var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(toUser);
            if (user is null)
            {
                throw new ArgumentNullException($"{toUser} doesnt exists.");
            }

            await JoinOrCreateChatInternal(user.ConnectionId, toUser, chatName);
            await Clients.OthersInGroup(chatName).SendTextToChat(chatName, fromUser, message);
        }
    }

    public async Task JoinOrCreateChat(string chatName)
    {
        var userName = Context.User.Identity.Name;
        await JoinOrCreateChatInternal(Context.ConnectionId, userName, chatName);

    }

    public async IAsyncEnumerable<HistoryMessageInfo> DownLoadHistory(string chatName)
    {
        var isExists = await _roomService.IsChatExists(chatName);
        if (isExists)
        {
            var list = await _roomService.GetGroupHistoryAsync(chatName);

            foreach (var senderInfo in list)
            {
                var message = Encoding.UTF8.GetString(senderInfo.MessageInfo.Message);
                yield return new HistoryMessageInfo(chatName, senderInfo.UserName, message);
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

    public async IAsyncEnumerable<SenderInfo> GetGroupHistory(string chatName, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var list = await _roomService.GetGroupHistoryAsync(chatName);

        foreach (var senderInfo in list)
        {
            yield return senderInfo;
            await Task.Delay(50);
        }
    }

    public async IAsyncEnumerable<byte[]> Downloads(HubDownloadInfo downloadInfo, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "test.jpeg");
        var bytes = File.ReadAllBytes(path);

        for (var i = 0; i < downloadInfo.Amount && cancellationToken.IsCancellationRequested == false; i++)
        {
            yield return bytes;
            await Task.Delay(200);
        }
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
