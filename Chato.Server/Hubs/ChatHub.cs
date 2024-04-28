using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chato.Server.Hubs;

public record HubDownloadInfo(int Amount);

public interface ITest
{
    Task ReceiveMessage(string user, string message);
}
public class ChatHub : Hub//<ITest>
{
    public const string TOPIC_MESSAGE_STRING_RECEIVED = "MessageStringRecieved";
    public const string TOPIC_MESSAGE_BYTE_RECEIVED = "MessageByteRecieved";

    public override async Task OnConnectedAsync()
    {
        var ptr = Encoding.UTF8.GetBytes("Your are connected");

        await SendMessageToOthers("server",ptr);
        await base.OnConnectedAsync();
    }


    public Task SendMessageToOthers(string user, byte []  message)
    {
        return Clients.Others.SendAsync(TOPIC_MESSAGE_STRING_RECEIVED, user, message);
    }


    public Task SendMessageToOthersInGroup(string group, string user, byte [] ptr)
    {
        //var message = Encoding.UTF8.GetkckString(ptr);
        return Clients.OthersInGroup(group).SendAsync(TOPIC_MESSAGE_STRING_RECEIVED, user, ptr);
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
