﻿using Chato.Server.DataAccess.Models;
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

//public class TextMessage
//{
//    [JsonPropertyName("chat")]
//    public string Chat { get; set; }

//    [JsonPropertyName("fromUser")]
//    public string FromUser { get; set; }

//    [JsonPropertyName("message")]
//    public string Message { get; set; }

//    public TextMessage(string chat, string fromUser, string message)
//    {
//        Chat = chat;
//        FromUser = fromUser;
//        Message = message;
//    }
//}




public interface IChatHub
{

    //Task SendTextToGroup(TextMessage textMessage);
    Task SendTextToGroup(string chat, string fromUser, string toUser,string message);
    Task SendText(string fromUser, string message);
    Task SelfReplay(string message);
}

[Authorize]
public class ChattoHub : Hub<IChatHub>
{
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

        await ReplyMessage("server", "You are connected");
        await base.OnConnectedAsync();
    }

    public Task ReplyMessage(string fromUser, string message)
    {
        return Clients.Caller.SendText(fromUser, message);
    }


    public Task BroadcastMessage(string fromUser, string message)
    {
        //var str = Encoding.UTF8.GetString(message);
        return Clients.Others.SendText(fromUser, message);
    }

    public async Task SendMessageToOthersInGroup(string chat, string fromUser,string toUser, string message)
    {
        //var ptr = Encoding.UTF8.GetBytes(message);
        await _roomService.SendMessageAsync(chat, fromUser, message);
        await Clients.OthersInGroup(chat).SendTextToGroup(chat,fromUser,toUser,message);
        //await Clients.OthersInGroup(chat).SendTextToGroup(new TextMessage(chat,fromUser,message));
    }

    public async Task SendMessageToOtherUser(string fromUser, string toUser, string ptr)
    {
        //var user = await _userRepository.GetAsync(x => x.UserName == toUser);
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(toUser);
        if (user is not null)
        {
            await Clients.Client(user.ConnectionId).SendText(fromUser, ptr);
        }
    }

    public async Task JoinGroup(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await _assignmentService.JoinGroup(Context.User.Identity.Name, roomName);
    }


    /// <summary>
    /// After registration.
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    public async Task JoinLobi()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, IAssignmentService.Lobi);
        await _assignmentService.JoinLobi(Context.User.Identity.Name);
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

    public async IAsyncEnumerable<SenderInfo> GetGroupHistory(string roomName, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var list = await _roomService.GetGroupHistoryAsync(roomName);

        foreach (var senderInfo in list)
        {
            yield return senderInfo;
            await Task.Delay(200);
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

}
