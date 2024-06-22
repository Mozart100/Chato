﻿using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chato.Server.Hubs;

public record HubDownloadInfo(int Amount);

public interface IChatHub
{
    Task SendMessage(string fromUser, byte[] message);
    //Task SendToUser(string fromUser, byte[] message);
}

public interface IChattobEndpoints
{
    Task BroadcastMessage(string fromUser, byte[] message);
    Task ReplyMessage(string fromUser, byte[] message);
    Task SendMessageToOtherUser(string fromUser, string toUser, byte[] ptr);


    IAsyncEnumerable<byte[]> Downloads(HubDownloadInfo downloadInfo, [EnumeratorCancellation] CancellationToken cancellationToken);
    IAsyncEnumerable<SenderInfo> GetGroupHistory(string roomName, [EnumeratorCancellation] CancellationToken cancellationToken);
    Task JoinGroup(string roomName);
    Task LeaveGroup(string groupName);
    Task OnConnectedAsync();
    Task RemoveChatHistory(string groupName);
    Task SendMessageToOthersInGroup(string group, string fromUser, byte[] ptr);
    
    Task UserDisconnectAsync();
}


[Authorize]
public class ChattoHub : Hub<IChatHub> , IChattobEndpoints
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


    public Task BroadcastMessage(string fromUser, byte[] message)
    {
        return Clients.Others.SendMessage(fromUser, message);
    }

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




    //public async IAsyncEnumerable<string> Download(HubDownloadInfo downloadInfo, [EnumeratorCancellation] CancellationToken cancellationToken)
    //{
    //    var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "css.txt");
    //    var bytes = File.ReadAllBytes(path);

    //    for (var i = 0; i < downloadInfo.Amount && cancellationToken.IsCancellationRequested == false; i++)
    //    {
    //        yield return $"Download - {i}";
    //        await Task.Delay(200);
    //    }
    //}
}