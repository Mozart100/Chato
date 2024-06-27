﻿using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Chato.Server.Hubs;

public record HubDownloadInfo(int Amount);


public interface IChatHub
{
    Task SendMessage(string fromUser, byte[] message);
    Task SendText(string fromUser, string message);
    Task SelfReplay(string message);

    //Task NotifyGroupInfo(string type , string message);
    //Task SendToUser(string fromUser, byte[] message);
}

public interface IChattobEndpoints
{
    Task BroadcastMessage(string fromUser, byte[] message);
    Task ReplyMessage(string fromUser, byte[] message);
    Task SendMessageToOtherUser(string fromUser, string toUser, byte[] ptr);

    //Task GetGroupInfo(string groupNamekc);

}


[Authorize]
public class ChattoHub : Hub<IChatHub>, IChattobEndpoints
{
    private readonly IUserService _userService;
    private readonly IRoomService _roomService;
    private readonly IAssignmentService _userRoomService;

    public ChattoHub(
        IUserService userService,
        IRoomService roomService,
        IAssignmentService userRoomService)
    {
        this._userService = userService;
        this._roomService = roomService;
        this._userRoomService = userRoomService;
    }


    public override async Task OnConnectedAsync()
    {
        var ptr = Encoding.UTF8.GetBytes("Your are connected");

        var connectionId = Context.ConnectionId;
        var user = Context.User;

        await _userService.AssignConnectionId(user.Identity.Name, connectionId);

        await ReplyMessage("server", ptr);
        await base.OnConnectedAsync();
    }

    public Task ReplyMessage(string fromUser, byte[] message)
    {
        return Clients.Caller.SendMessage(fromUser, message);
    }

    public Task SendAll(string fromUser, string message)
    {
        return Clients.Others.SendText(fromUser, message);
    }


    public Task BroadcastMessage(string fromUser, byte[] message)
    {
        var str = Encoding.UTF8.GetString(message);
        return Clients.Others.SendMessage(fromUser, message);
    }

    //public async Task GetGroupInfo(string groupName)
    //{
    //    var roomInfo = await _roomService.GetRoomByNameOrIdAsync(groupName);

    //    var serialized = string.Empty;

    //    if (roomInfo is not null)
    //    {
    //        serialized = JsonSerializer.Serialize(roomInfo);
    //    }
    //    else
    //    {
    //        serialized = JsonSerializer.Serialize(ChatRoomDto.Empty());
    //    }

    //    await Clients.Caller.SelfReplay(serialized);
    //}

    public async Task SendMessageToOthersInGroup(string group, string fromUser, byte[] ptr)
    {
        //var message = Encoding.UTF8.GetkckString(ptr);

        await _roomService.SendMessageAsync(group, fromUser, ptr);
        await Clients.OthersInGroup(group).SendMessage(fromUser, ptr);
    }

    public async Task SendMessageToOtherUser(string fromUser, string toUser, byte[] ptr)
    {
        //var user = await _userRepository.GetAsync(x => x.UserName == toUser);
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(toUser);
        if (user is not null)
        {
            await Clients.Client(user.ConnectionId).SendMessage(fromUser, ptr);
        }
    }

    public async Task JoinGroup(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await _userRoomService.JoinGroupByConnectionId(Context.User.Identity.Name, roomName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await _userRoomService.RemoveUserByUserNameOrIdAsync(Context.User.Identity.Name);
    }


    public async Task UserDisconnectAsync()
    {
        await OnDisconnectedAsync(null);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        await _userRoomService.RemoveUserByUserNameOrIdAsync(Context.User.Identity.Name);
    }


    public async Task RemoveChatHistory(string groupName)
    {
        await _roomService.RmoveHistoryByRoomNameAsync(groupName);

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
