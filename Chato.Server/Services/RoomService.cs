using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.QueueDelegates;
using Chatto.Shared;

namespace Chato.Server.Services;


public interface IRoomService
{
    Task AddUserAsync(string roomName, string userName);
    Task<ChatRoomDto> CreateRoomAsync(string roomName);
    Task<ChatRoomDto[]> GetAllRoomAsync();
    Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName);
    Task<ChatRoomDto?> GetRoomByNameOrIdAsync(string nameOrId);
    Task JoinOrCreateRoom(string roomName, string userName);
    Task RemoveRoomByNameOrIdAsync(string nameOrId);
    Task RemoveUserAndRoomFromRoom(string roomName, string username);
    Task RemoveHistoryByRoomNameAsync(string roomName);
    Task SendMessageAsync(string group, string fromUser, byte[] ptr);
}


public class RoomService : IRoomService
{
    private readonly IRoomRepository _chatRoomRepository;
    private readonly ILockerDelegateQueue _delegateQueue;
    private readonly IRoomIndexerRepository _roomIndexerRepository;

    public RoomService(IRoomRepository chatRoomRepository, ILockerDelegateQueue delegateQueue, IRoomIndexerRepository roomIndexerRepository)
    {
        this._chatRoomRepository = chatRoomRepository;
        this._delegateQueue = delegateQueue;
        this._roomIndexerRepository = roomIndexerRepository;
    }

    public async Task AddUserAsync(string roomName, string userName)
    {
        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await AddUserCoreAsync(roomName, userName);
        });
    }

    private async Task<ChatRoomDb> AddUserCoreAsync(string roomName, string userName)
    {
        var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);

        if (room is not null)
        {
            room.Users.Add(userName);
        }
        return room;
    }

    public async Task<ChatRoomDto> CreateRoomAsync(string roomName)
    {
        var result = default(ChatRoomDb);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await CreateRoomCoreAsync(roomName);
        });

        return result.ToChatRoomDto();
    }

    private async Task<ChatRoomDb> CreateRoomCoreAsync(string roomName)
    {
        return await _chatRoomRepository.InsertAsync(new ChatRoomDb { Id = roomName });
    }


    public async Task<ChatRoomDto[]> GetAllRoomAsync()
    {
        ChatRoomDb[] result = null;

        await _delegateQueue.InvokeAsync(async () =>
        {
            var list = await _chatRoomRepository.GetAllAsync();
            result = list.ToArray();
        });
        return result.SafeSelect(x => x.ToChatRoomDto()).SafeToArray();
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

    public async Task<ChatRoomDto> GetRoomByNameOrIdAsync(string nameOrId)
    {
        var result = default(ChatRoomDb);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await GetRoomByNameOrIdCoreAsync(nameOrId);

        });

        return result?.ToChatRoomDto();
    }

    private async Task<ChatRoomDb> GetRoomByNameOrIdCoreAsync(string nameOrId)
    {
        return await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == nameOrId);
    }

    public async Task RemoveRoomByNameOrIdAsync(string nameOrId)
    {
        await _delegateQueue.InvokeAsync(async () => await RemoveRoomByNameOrIdCoreAsync(nameOrId));
        await _roomIndexerRepository.RemoveAsync(nameOrId);
        //await _cacheQueue.InvokeAsync(async () => await _chatRoomRepository.RemoveAsync(x => x.RoomName == nameOrId));
    }


    private async Task RemoveRoomByNameOrIdCoreAsync(string nameOrId)
    {
        await _chatRoomRepository.RemoveAsync(x => x.RoomName == nameOrId);
    }

    public async Task RemoveHistoryByRoomNameAsync(string roomName)
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

    public async Task RemoveUserAndRoomFromRoom(string roomName, string username)
    {
        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            room.Users.Remove(username);

            if (room.Users.Any() == false)
            {
                await RemoveRoomByNameOrIdCoreAsync(roomName);
            }
        });
    }

    public async Task JoinOrCreateRoom(string roomName, string userName)
    {
        await _delegateQueue.InvokeAsync(async () =>
        {
            var room = await GetRoomByNameOrIdCoreAsync(roomName);
            if (room is not null)
            {
                room.Users.Add(userName);
            }
            else
            {
                var createRoom = await CreateRoomCoreAsync(roomName);
                createRoom.Users.Add(userName);
            }
        });
    }
}
