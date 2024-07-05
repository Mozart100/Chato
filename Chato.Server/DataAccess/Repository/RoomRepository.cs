using Chato.Server.DataAccess.Models;
using Chatto.Shared;
using System.Reflection;

namespace Chato.Server.DataAccess.Repository;

public interface IRoomRepository : IRepositoryBase<ChatRoomDb>
{
    Task SendMessageAsync(string group, string user, byte[] ptr);
}

public class RoomRepository : RepositoryBase<ChatRoomDb>, IRoomRepository
{
    private readonly ILogger<RoomRepository> _logger;

    public RoomRepository(ILogger<RoomRepository> logger)
    {
        _logger = logger;
    }

    public async Task SendMessageAsync(string groupName, string user, byte[] ptr)
    {
        var chatRoom = await GetOrDefaultAsync(x => x.Id == groupName);

        if (chatRoom is null)
        {
            chatRoom = Insert(new ChatRoomDb { Id = groupName });
        }

        chatRoom.SenderInfo.Add(new SenderInfo(user, ptr));

    }
}
