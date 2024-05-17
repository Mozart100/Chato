using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;

namespace Chato.Server.Services;


public interface IRoomService
{
    Task AddUserAsync(string roomName, string userName);
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
    private readonly IDelegateQueue _delegateQueue;

    public RoomService(IRoomRepository chatRoomRepository, IDelegateQueue delegateQueue)
    {
        this._chatRoomRepository = chatRoomRepository;
        this._delegateQueue = delegateQueue;
    }

    public async Task AddUserAsync(string roomName, string userName)
    {
        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);

            if (room is not null)
            {
                room.Users.Add(userName);
            }
        });

    }

    public async Task<ChatRoomDb> CreateRoomAsync(string roomName)
    {
        var result = default(ChatRoomDb);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _chatRoomRepository.InsertAsync(new ChatRoomDb { Id = roomName });
        });
        return result;
    }

    public async Task<ChatRoomDb[]> GetAllRoomAsync()
    {
        ChatRoomDb[] result = null;

        await _delegateQueue.InvokeAsync(async () =>
        {
            var list = await _chatRoomRepository.GetAllAsync();
            result = list.ToArray();
        });
        return result;
    }

    public async Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName)
    {
        var result = Enumerable.Empty<SenderInfo>();

        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                result = room.SenderInfo.ToArray();
            }
        });

        return result;
    }

    public async Task<ChatRoomDb> GetRoomByNameOrIdAsync(string nameOrId)
    {
        var result = default(ChatRoomDb);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == nameOrId);

        });
        return result;
    }


    public async Task RemoveRoomByNameOrIdAsync(string nameOrId)
    {
        await _delegateQueue.InvokeAsync(async () => await _chatRoomRepository.RemoveAsync(x => x.RoomName == nameOrId));
    }

    public async Task RmoveHistoryByRoomNameAsync(string roomName)
    {
        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                room.SenderInfo.Clear();
            }
        });
    }

    public async Task SendMessageAsync(string roomName, string fromUser, byte[] ptr)
    {
        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                room.SenderInfo.Add(new SenderInfo(fromUser, ptr));

            }
        });
    }
}
