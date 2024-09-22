using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.QueueDelegates;
using Chatto.Shared;
using System.Net.NetworkInformation;
using System.Text;

namespace Chato.Server.Services;


public interface IChatService
{
    public const string Lobi = "lobi";
    public static string GetToUser(string chatName) => chatName.Split("__").LastOrDefault();
    static string GetChatName(string fromUser, string toUser) => $"{fromUser}__{toUser}";


    Task AddUserAsync(string roomName, string userName);
    Task<ChatRoomDto> CreateRoomAsync(string roomName);
    Task<ChatRoomDto[]> GetAllRoomAsync();
    Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName);
    Task<ChatRoomDto?> GetRoomByNameOrIdAsync(string nameOrId);
    Task JoinOrCreateRoom(string roomName, string userName);
    Task CreateLobi();
    Task RemoveRoomByNameOrIdAsync(string nameOrId);
    Task RemoveUserAndRoomFromRoom(string roomName, string username);
    Task RemoveHistoryByRoomNameAsync(string roomName);
    Task SendMessageAsync(string group, string fromUser, string message);

    Task<bool> IsChatExists(string chatName);
}


public class ChatService : IChatService
{
    private readonly IChatRepository _chatRoomRepository;
    private readonly ILockerDelegateQueue _lockerQueue;

    public ChatService(IChatRepository chatRoomRepository,
        ILockerDelegateQueue lockerQueue)
    {
        this._chatRoomRepository = chatRoomRepository;
        this._lockerQueue = lockerQueue;
    }

    public async Task AddUserAsync(string roomName, string userName)
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await AddUserCoreAsync(roomName, userName);
        });
    }

    private async Task<ChatDb> AddUserCoreAsync(string roomName, string userName)
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
        var result = default(ChatDb);

        await _lockerQueue.InvokeAsync(async () =>
        {
            result = await CreateRoomCoreAsync(roomName);
        });

        return result.ToChatRoomDto();
    }

    private async Task<ChatDb> CreateRoomCoreAsync(string roomName)
    {
        var result = await _chatRoomRepository.InsertAsync(new ChatDb { Id = roomName });
        return result;
    }


    public async Task<ChatRoomDto[]> GetAllRoomAsync()
    {
        ChatDb[] result = null;

        await _lockerQueue.InvokeAsync(async () =>
        {
            var list = await _chatRoomRepository.GetAllAsync();
            result = list.ToArray();
        });
        return result.SafeSelect(x => x.ToChatRoomDto()).SafeToArray();
    }

    public async Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName)
    {
        var result = Enumerable.Empty<SenderInfo>();

        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                result = room.Messages.SafeToArray();
            }
        });

        return result;
    }

    public async Task<ChatRoomDto> GetRoomByNameOrIdAsync(string nameOrId)
    {
        var result = default(ChatDb);

        await _lockerQueue.InvokeAsync(async () =>
        {
            result = await GetRoomByNameOrIdCoreAsync(nameOrId);

        });

        return result?.ToChatRoomDto();
    }

    private async Task<ChatDb> GetRoomByNameOrIdCoreAsync(string nameOrId)
    {
        return await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == nameOrId);
    }

    public async Task RemoveRoomByNameOrIdAsync(string nameOrId)
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            await RemoveRoomByNameOrIdCoreAsync(nameOrId);
        });
    }

    private async Task RemoveRoomByNameOrIdCoreAsync(string nameOrId)
    {
        await _chatRoomRepository.RemoveAsync(x => x.RoomName == nameOrId);
    }

    public async Task RemoveHistoryByRoomNameAsync(string roomName)
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                room.Messages.Clear();
            }
        });
    }

    public async Task SendMessageAsync(string roomName, string fromUser, string message)
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                var ptr = Encoding.UTF8.GetBytes(message);
                room.Messages.Add(new SenderInfo(fromUser, ptr, IsText: true));
            }
        });
    }

    public async Task RemoveUserAndRoomFromRoom(string roomName, string username)
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            if (room is not null)
            {
                room.Users.Remove(username);

                if (room.Users.Any() == false)
                {
                    await RemoveRoomByNameOrIdCoreAsync(roomName);
                }
            }
        });
    }

    public async Task JoinOrCreateRoom(string roomName, string userName)
    {
        await _lockerQueue.InvokeAsync(async () =>
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

    public async Task CreateLobi()
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await GetRoomByNameOrIdCoreAsync(IChatService.Lobi);
            if (room is null)
            {
                await CreateRoomCoreAsync(IChatService.Lobi);
            }
        });
    }

    public async Task<bool> IsChatExists(string chatName)
    {
        var result = true;

        await _lockerQueue.InvokeAsync(async () =>
        {
            var chat = await GetRoomByNameOrIdCoreAsync(chatName);

            result = chat != null;

        });

        return result;
    }
}
