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
    private readonly IRoomIndexerRepository _roomIndexerRepository;

    public RoomRepository(ILogger<RoomRepository> logger, IRoomIndexerRepository roomIndexerRepository)
    {
        _logger = logger;
        this._roomIndexerRepository = roomIndexerRepository;
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


    public override ChatRoomDb Insert(ChatRoomDb instance)
    {
        if (Models.Add(instance) == true)
        {
            _roomIndexerRepository.AddToCache(instance.RoomName);
            return instance;
        }

        throw new Exception("Key already present.");

    }


    public async override Task<bool> RemoveAsync(Predicate<ChatRoomDb> selector)
    {
        var result = false;
        foreach (var model in Models)
        {
            if (selector(model))
            {
                result = Models.Remove(model);
                if(result == true)
                {
                    _roomIndexerRepository.Remove(model.RoomName);
                }
            }
        }

        return result;
    }
}

