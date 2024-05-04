using Chato.Server.DataAccess.Models;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRoomRepository : IRepositoryBase<ChatRoomDb>
{
    //Task AddSenderInfoAsync(string chatRoomId, SenderInfo senderInfo);
    Task CreateOrAndAsync(string group, string user, byte[] ptr);
    //Task<ChatRoomDb> GetOrAdd(string chatRoomId);
}

public class ChatRoomRepository : RepositoryBase<ChatRoomDb>, IChatRoomRepository
{
    private readonly ILogger<ChatRoomRepository> _logger;

    public ChatRoomRepository(ILogger<ChatRoomRepository> logger)
    {
        _logger = logger;
    }

    //public async Task AddSenderInfoAsync(string chatRoomId, SenderInfo senderInfo)
    //{
    //    var chatRoom = await GetAsync(x => x.Id == chatRoomId);

    //    if (chatRoom is not null)
    //    {
    //        chatRoom.SenderInfo.Add(senderInfo);
    //    }
    //}

    public async Task CreateOrAndAsync(string groupName, string user, byte[] ptr)
    {
        var chatRoom = await GetAsync(x => x.Id == groupName);

        if (chatRoom is null)
        {
            chatRoom = Insert(new ChatRoomDb { Id = groupName });
        }

        chatRoom.SenderInfo.Add(new SenderInfo(user, ptr));

    }

    //public async Task<ChatRoomDb> GetOrAdd(string chatRoomId)
    //{

    //    var chatRoom = await GetAsync(x => x.Id == chatRoomId);

    //    if (chatRoom is not null)
    //    {
    //        return chatRoom;
    //    }

    //    return new ChatRoomDb { Id = chatRoomId };
    //}

}