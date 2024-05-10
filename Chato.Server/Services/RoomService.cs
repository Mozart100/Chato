using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;

namespace Chato.Server.Services;


public interface IRoomService
{
    Task<ChatRoomDb[]> GetAllRoomAsync();
}

public class RoomService : IRoomService
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public RoomService(IChatRoomRepository chatRoomRepository)
    {
        this._chatRoomRepository = chatRoomRepository;
    }

    public async Task<ChatRoomDb[]> GetAllRoomAsync()
    {
       var result = await  _chatRoomRepository.GetAllAsync();
        return result.ToArray();
    }
}
