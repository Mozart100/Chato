using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;

namespace Chato.Server.Services;


public interface IRoomService
{
    Task AddUserAsync(string roomName , string userName);
    Task<ChatRoomDb> CreateRoomAsync(string roomName);
    Task<ChatRoomDb[]> GetAllRoomAsync();
    Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName);
    Task<ChatRoomDb> GetRoomByNameOrIdAsync(string nameOrId);
    Task RemoveRoomByNameOrIdAsync(string nameOrId);
    Task RmoveHistoryByRoomNameAsync(string roomName);
    Task SendMessageAsync(string group, string fromUser, byte[] ptr);
}

public class RoomService : IRoomService
{
    private readonly IRoomRepository _chatRoomRepository;

    public RoomService(IRoomRepository chatRoomRepository)
    {
        this._chatRoomRepository = chatRoomRepository;
    }

    public async Task AddUserAsync(string roomName, string userName)
    {
        var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);

        if (room is not null)
        {
            room.Users.Add(userName);
        }
    }

    public async Task<ChatRoomDb> CreateRoomAsync(string roomName)
    {
        var result = await _chatRoomRepository.InsertAsync(new ChatRoomDb { Id = roomName});
        return result;
    }

    public async Task<ChatRoomDb[]> GetAllRoomAsync()
    {
        var result = await _chatRoomRepository.GetAllAsync();
        return result.ToArray();
    }

    public async Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName)
    {
        var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);

        if (room is not null)
        {
            return room.SenderInfo.ToArray();
        }

        return Enumerable.Empty<SenderInfo>();
    }

    public async Task<ChatRoomDb> GetRoomByNameOrIdAsync(string nameOrId)
    {
        var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == nameOrId);
        return room;
    }


    public async Task RemoveRoomByNameOrIdAsync(string nameOrId)
    {
        var room = await _chatRoomRepository.RemoveAsync(x => x.GroupName == nameOrId);
    }

    public async Task RmoveHistoryByRoomNameAsync(string roomName)
    {
        var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
        if (room is not null)
        {
            room.SenderInfo.Clear();
        }
    }

    public async Task SendMessageAsync(string roomName, string fromUser, byte[] ptr)
    {
        var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
        if (room is not null)
        {
            room.SenderInfo.Add(new SenderInfo(fromUser, ptr));

        }
    }
}
