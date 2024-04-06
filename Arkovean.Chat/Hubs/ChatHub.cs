﻿using Microsoft.AspNetCore.SignalR;

namespace Arkovean.Chat.Hubs;

public class ChatHub : Hub
{
    public const string TOPIC_MESSAGE_RECEIVED = "MessageRecieved";

    public override async Task OnConnectedAsync()
    {
        await SendMessageAllUsers(string.Empty, "Your are connected");
        await base.OnConnectedAsync();
    }

    public Task SendMessageAllUsers(string user, string nessage)
    {
       return Clients.All.SendAsync(TOPIC_MESSAGE_RECEIVED, user, nessage);
    }
}