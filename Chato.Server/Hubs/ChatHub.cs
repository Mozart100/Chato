using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
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

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IUserRepository _userRepository;

    public ChatHub(IChatRoomRepository chatRoomRepository, IUserRepository userRepository)
    {
        this._chatRoomRepository = chatRoomRepository;
        this._userRepository = userRepository;
    }


    public override async Task OnConnectedAsync()
    {
        var ptr = Encoding.UTF8.GetBytes("Your are connected");

        var comnectionId = Context.ConnectionId;
        var user = Context.User;

        await _userRepository.InsertAsync(new UserDb { UserName = user.Identity.Name, ConnectionId = comnectionId });

        await SendMessageToOthers("server", ptr);
        await base.OnConnectedAsync();
    }


    public Task SendMessageToOthers(string user, byte[] message)
    {
        return Clients.Others.SendMessage(user, message);
    }

    public async Task SendMessageToOthersInGroup(string group, string user, byte[] ptr)
    {
        //var message = Encoding.UTF8.GetkckString(ptr);

        await _chatRoomRepository.CreateOrAndAsync(group, user, ptr);
        await Clients.OthersInGroup(group).SendMessage(user, ptr);
    }

    public async Task SendMessageToOtherUser(string fromUser, string toUser, byte[] ptr)
    {
        var user = await _userRepository.GetAsync(x => x.UserName == toUser);
        await Clients.Client(user.ConnectionId).SendMessage(fromUser, ptr);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task RemoveChatHistory(string groupName)
    {
        var group = await _chatRoomRepository.GetAsync(x => x.Id == groupName);
        if (group is not null)
        {
            group.SenderInfo.Clear();
        }
    }

    public async IAsyncEnumerable<SenderInfo> GetGroupHistory(string groupName, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var group = await _chatRoomRepository.GetAsync(x => x.Id == groupName);

        if (group is not null)
        {
            foreach (var senderInfo in group.SenderInfo.ToArray())
            {
                yield return senderInfo;
                await Task.Delay(200);
            }
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
