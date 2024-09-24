using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Chatto.Shared;
using System.Reflection;
using System.Text;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRepository : IRepositoryBase<ChatDb>
{
    Task SendMessageAsync(string group, string user, string message);
}

public class ChatRepository : RepositoryBase<ChatDb>, IChatRepository
{
    private readonly ILogger<ChatRepository> _logger;
    private readonly IRoomIndexerRepository _roomIndexerRepository;

    public ChatRepository(ILogger<ChatRepository> logger, IRoomIndexerRepository roomIndexerRepository)
    {
        _logger = logger;
        this._roomIndexerRepository = roomIndexerRepository;
    }

    public async Task SendMessageAsync(string chatName, string user, string message)
    {
        var chatRoom = await GetOrDefaultAsync(x => x.Id == chatName);

        if (chatRoom is null)
        {
            chatRoom = Insert(new ChatDb { Id = chatName });
        }

        var ptr = AuthenticationService.GetBytes(message);
        chatRoom.Messages.Add(new SenderInfo(user, MessageInfo: new UserMessageInfo(ptr, UserMessageType.String)));

    }

    public override IEnumerable<ChatDb> GetAll()
    {
        var rooms = base.GetAll();
        foreach (var item in rooms)
        {
            _roomIndexerRepository.AddToCache(item.RoomName);
            
        };

        return rooms;
    }

    public override ChatDb Insert(ChatDb instance)
    {
        if (Models.Add(instance) == true)
        {
            _roomIndexerRepository.AddToCache(instance.RoomName);
            return instance;
        }

        throw new Exception("Key already present.");

    }

    protected override ChatDb CoreGet(Predicate<ChatDb> selector)
    {
        var result =  base.CoreGet(selector);
    
        if(result is not null)
        {
            _roomIndexerRepository.AddToCache(result.RoomName);
        }

        return result;
    }


    public async override Task<bool> RemoveAsync(Predicate<ChatDb> selector)
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

