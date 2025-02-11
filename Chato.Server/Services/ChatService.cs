using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.QueueDelegates;
using Chatto.Shared;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Chato.Server.Services;


public interface IChatService
{
    public const string Lobi = "lobi";
    public const string ChatImages = "ChattoImages";
    public static string GetToUser(string chatName) => chatName.Split("__").LastOrDefault();
    public static string GetChatName(string fromUser, string toUser) => $"{fromUser}__{toUser}";


    Task AddUserAsync(string roomName, string userName);
    Task<ChatRoomDto> CreateRoomAsync(string roomName, ChatType chatType);
    Task<ChatRoomDto[]> GetAllRoomAsync();
    Task<ChatRoomDto[]> GetChatsAsync(Func<ChatDb, bool> predicate);
    Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName);
    Task<ChatRoomDto?> GetRoomByNameOrIdAsync(string nameOrId);
    Task<SenderInfo> JoinOrCreateRoom(string roomName, string userName, ChatType chatType);
    Task CreateLobi();
    Task RemoveRoomByNameOrIdAsync(string nameOrId);
    Task RemoveUserAndRoomFromRoom(string roomName, string username);
    Task RemoveHistoryByRoomNameAsync(string roomName);
    Task<SenderInfo> SendMessageAsync(string roomName, string fromUser, string? textMessage, string? image, SenderInfoType messageType);
    Task<bool> IsChatExists(string chatName);
    Task<IEnumerable<ChatInfoPerUser>> GetChatInfoPerChatName(IEnumerable<string> chats);

}


public class ChatService : IChatService
{
    private readonly IChatRepository _chatRoomRepository;
    private readonly ILockerDelegateQueue _lockerQueue;
    private readonly IWebHostEnvironment _env;

    public ChatService(IChatRepository chatRoomRepository,
        ILockerDelegateQueue lockerQueue,
        IWebHostEnvironment env)
    {
        this._chatRoomRepository = chatRoomRepository;
        this._lockerQueue = lockerQueue;
        this._env = env;
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

    public async Task<ChatRoomDto> CreateRoomAsync(string roomName, ChatType chatType)
    {
        var result = default(ChatDb);

        await _lockerQueue.InvokeAsync(async () =>
        {
            result = await CreateRoomCoreAsync(roomName, chatType);
        });

        return result.ToChatRoomDto();
    }

    private async Task<ChatDb> CreateRoomCoreAsync(string roomName, ChatType chatType)
    {
        var result = await _chatRoomRepository.InsertAsync(new ChatDb { Id = roomName, ChatType = chatType });
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

    public async Task<ChatRoomDto[]> GetChatsAsync(Func<ChatDb, bool> predicate)
    {
        ChatDb[] result = null;

        await _lockerQueue.InvokeAsync(async () =>
        {
            var list = await _chatRoomRepository.GetAllAsync();
            result = list.Where(x => predicate(x)).ToArray();
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

    public async Task<IEnumerable<ChatInfoPerUser>> GetChatInfoPerChatName(IEnumerable<string> chats)
    {
        var result = new List<ChatInfoPerUser>();

        await _lockerQueue.InvokeAsync(async () =>
        {
            foreach (var chat in chats)
            {
                var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == chat);
                if (room is not null)
                {
                    result.Add(new ChatInfoPerUser(room.RoomName, room.ChatType, room.Users.Count));
                }
            }
        });

        return result;
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

    public async Task<SenderInfo> SendMessageAsync(string chatName, string fromUser, string? textMessage, string? imageName, SenderInfoType messagetype)
    {
        var result = default(SenderInfo);


        if (messagetype == SenderInfoType.TextMessage)
        {
            await _lockerQueue.InvokeAsync(async () =>
            {
                result = await AddTextMessage(SenderInfoType.TextMessage, chatName, fromUser, textMessage, imageName);
            });
        }
        else
        {
            if (messagetype == SenderInfoType.Image)
            {

                await _lockerQueue.InvokeAsync(async () =>
                {

                    var chatRoom = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == chatName);
                    if (chatRoom is not null)
                    {

                        var amountMessages = chatRoom.Messages.Count + 1;
                        //var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", IChatService.ChatImages, chatName);
                        var localPath = $"{amountMessages}{Path.GetExtension(imageName)}";


                        //if (!Directory.Exists(wwwRootPath))
                        //{
                        //    Directory.CreateDirectory(wwwRootPath);
                        //}

                        var wwwRootPath = Path.Combine(_env.WebRootPath, IChatService.ChatImages, chatName);
                        if (Directory.Exists(wwwRootPath) == false)
                        {
                            Directory.CreateDirectory(wwwRootPath);
                        }

                        byte[] fileBytes = Convert.FromBase64String(textMessage);
                        var filePath = Path.Combine(wwwRootPath, localPath);

                        try
                        {
                            File.WriteAllBytes(filePath, fileBytes);
                        }
                        catch (Exception ex)
                        {

                        }

                        var imageFilePath = $"{IChatService.ChatImages}/{chatName}/{localPath}";
                        var senderinfo = new SenderInfo(SenderInfoType.Image, fromUser, textMessage, imageFilePath, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                        chatRoom.Messages.Add(senderinfo);

                        result = senderinfo with
                        {
                            Image = imageFilePath,
                            TextMessage = null
                        };
                    }

                });

            }
        }

        return result;
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

    public async Task<SenderInfo> JoinOrCreateRoom(string roomName, string userName, ChatType chatType)
    {
        var result = default(SenderInfo);

        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await GetRoomByNameOrIdCoreAsync(roomName);
            if (room is not null)
            {
                if (room.ContainUser(userName) == false)
                {
                    room.Users.Add(userName);
                    result = await AddTextMessage(SenderInfoType.Joined, roomName, userName, null, null);
                }
            }
            else
            {
                var createRoom = await CreateRoomCoreAsync(roomName, chatType);
                createRoom.Users.Add(userName);
                result = await AddTextMessage(SenderInfoType.Created, roomName, userName, null, null);
            }
        });

        return result;
    }

    public async Task CreateLobi()
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await GetRoomByNameOrIdCoreAsync(IChatService.Lobi);
            if (room is null)
            {
                await CreateRoomCoreAsync(IChatService.Lobi, ChatType.Public);
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


    //----------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------

    private async Task<SenderInfo> AddTextMessage(SenderInfoType senderInfoType, string chatName, string fromUser, string? textMessage, string? image)
    {
        var result = default(SenderInfo);

        var chatRoom = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == chatName);
        result = chatRoom.AddTextMessage(senderInfoType, fromUser, textMessage, image);

        return result;
    }


}
