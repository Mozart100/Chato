using Chato.Server.DataAccess.Models;

namespace Chato.Server.DataAccess.Repository;

public interface IChatRoomRepository : IRepositoryBase<ChatRoomDb>
{

}

public class ChatRoomRepository : RepositoryBase<ChatRoomDb>, IChatRoomRepository
{
    private readonly ILogger<ChatRoomRepository> _logger;

    public ChatRoomRepository(ILogger<ChatRoomRepository> logger)
    {
        _logger = logger;
    }

   
}