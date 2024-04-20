using Microsoft.AspNetCore.SignalR;

namespace Chato.Server.Hubs;

public class ChatHub : Hub
{
    public const string TOPIC_MESSAGE_RECEIVED = "MessageRecieved";

    public override async Task OnConnectedAsync()
    {
        await SendMessageToOthers("server", "Your are connected");
        await base.OnConnectedAsync();
    }

    public Task SendMessageToOthers(string user, string nessage)
    {
        return Clients.Others.SendAsync(TOPIC_MESSAGE_RECEIVED, user, nessage);
    }

    public Task SendMessageToOthersInGroup(string group,string user, string nessage)
    {
        return Clients.OthersInGroup(group).SendAsync(TOPIC_MESSAGE_RECEIVED, user, nessage);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
