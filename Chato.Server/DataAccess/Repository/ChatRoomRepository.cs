using Chato.Server.DataAccess.Models;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRoomRepository : IRepositoryBase<ChatRoomDb>
{
    Task CreateOrAndAsync(string group, string user, byte[] ptr);
}

public class ChatRoomRepository : RepositoryBase<ChatRoomDb>, IChatRoomRepository
{
    private readonly ILogger<ChatRoomRepository> _logger;

    public ChatRoomRepository(ILogger<ChatRoomRepository> logger)
    {
        _logger = logger;
    }

    public async Task CreateOrAndAsync(string groupName, string user, byte[] ptr)
    {
        var chatRoom = await GetOrDefaultAsync(x => x.Id == groupName);

        if (chatRoom is null)
        {
            chatRoom = Insert(new ChatRoomDb { Id = groupName });
        }

        chatRoom.SenderInfo.Add(new SenderInfo(user, ptr));

    }
}