using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace Chato.Server.Hubs;

public record HubDownloadInfo(int Amount);


public class ChatHub : Hub
{
    public const string TOPIC_MESSAGE_STRING_RECEIVED = "MessageStringRecieved";
    public const string TOPIC_MESSAGE_BYTE_RECEIVED = "MessageByteRecieved";

    public override async Task OnConnectedAsync()
    {
        await SendMessageToOthers("server", "Your are connected");
        await base.OnConnectedAsync();
    }

    public Task SendMessageToOthers(string user, string nessage)
    {
        return Clients.Others.SendAsync(TOPIC_MESSAGE_STRING_RECEIVED, user, nessage);
    }

    public Task SendMessageToOthersInGroup(string group, string user, string nessage)
    {
        return Clients.OthersInGroup(group).SendAsync(TOPIC_MESSAGE_STRING_RECEIVED, user, nessage);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }


    public async IAsyncEnumerable<byte[]> Downloads(HubDownloadInfo downloadInfo, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "css.txt");
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
