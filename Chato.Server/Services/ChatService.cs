﻿using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services.Validations;
using Chatto.Shared;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Chato.Server.Services;

public interface IChatService
{
    public const string ChatName = "chatName";
    public const string Lobi = "lobi";
    public const string ChatImages = "ChatImages";
    public const string ChatUploadedImagesTemplate = $"{ChatImages}" + "\\{0}\\uploaded";
    public static string GetToUser(string chatName) => chatName.Split("__").LastOrDefault();
    public static string GetChatName(string fromUser, string toUser) => $"{fromUser}__{toUser}";

    Task<ChatDto> CreateRoomAsync(string roomName, ChatType chatType, string description);
    Task<ChatDto[]> GetAllRoomAsync();
    //Task<ChatDto[]> GetChatsAsync(Func<Chat, bool> predicate);
    Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName);
    Task<ChatDto?> GetRoomByNameOrIdAsync(string nameOrId);
    Task<SenderInfo> JoinOrCreateRoom(string roomName, string userName, ChatType chatType, string description);
    Task CreateLobi();
    Task RemoveRoomByNameOrIdAsync(string nameOrId);
    Task RemoveUserAndRoomFromRoom(string roomName, string username);
    Task RemoveHistoryByRoomNameAsync(string roomName);
    Task<SenderInfo> SendMessageAsync(string roomName, string fromUser, string? textMessage, string? image, SenderInfoType messageType);
    Task<bool> IsChatExists(string chatName);
    Task<IEnumerable<ChatInfoPerUser>> GetChatInfoPerChatName(IEnumerable<ParticipantInChat> chats);

    Task<IEnumerable<string>> UploadFilesAsync(UserDto user, string chatName, IEnumerable<IFormFile> documents);

}


public class ChatService : IChatService
{
    private readonly IChatRepository _chatRoomRepository;
    private readonly IChatValidationService _chatValidationService;
    private readonly ILockerDelegateQueue _lockerQueue;
    private readonly IWebHostEnvironment _env;

    public ChatService(IChatRepository chatRoomRepository,
        IChatValidationService chatValidationService,
        ILockerDelegateQueue lockerQueue,
        IWebHostEnvironment env)
    {
        this._chatRoomRepository = chatRoomRepository;
        this._chatValidationService = chatValidationService;
        this._lockerQueue = lockerQueue;
        this._env = env;
    }


    public async Task<ChatDto> CreateRoomAsync(string roomName, ChatType chatType, string descrion)
    {
        var result = default(ChatDto);

        await _lockerQueue.InvokeAsync(async () =>
        {
            result = await CreateRoomCoreAsync(roomName, chatType, descrion);
        });

        return result;
    }

    private async Task<ChatDto> CreateRoomCoreAsync(string roomName, ChatType chatType, string description)
    {
        var result = await _chatRoomRepository.InsertAsync(new Chat { Id = roomName, ChatType = chatType, Description = description, Expire = DateTime.UtcNow.AddDays(1) });
        return result;
    }


    public async Task<ChatDto[]> GetAllRoomAsync()
    {
        ChatDto[] result = null;

        await _lockerQueue.InvokeAsync(async () =>
        {
            var list = await _chatRoomRepository.GetAllAsync();
            result = list.ToArray();
        });
        return result;
    }

    //public async Task<ChatDto[]> GetChatsAsync(Func<Chat, bool> predicate)
    //{
    //    ChatDto[] result = null;

    //    await _lockerQueue.InvokeAsync(async () =>
    //    {
    //        var list = await _chatRoomRepository.GetAllAsync();
    //        result = list.Where(x => predicate(x)).ToArray();
    //    });
    //    return result;
    //}


    public async Task<IEnumerable<SenderInfo>> GetGroupHistoryAsync(string roomName)
    {
        var result = Enumerable.Empty<SenderInfo>();

        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = _chatRoomRepository.FirstOrDefualt(x => x.Id == roomName);
            if (room is not null)
            {
                result = room.Messages;
            }
        });

        return result;
    }

    public async Task<ChatDto> GetRoomByNameOrIdAsync(string nameOrId)
    {
        var result = default(ChatDto);

        await _lockerQueue.InvokeAsync(async () =>
        {
            result = await GetRoomByNameOrIdCoreAsync(nameOrId);

        });

        return result;
    }

    public async Task<IEnumerable<ChatInfoPerUser>> GetChatInfoPerChatName(IEnumerable<ParticipantInChat> chats)
    {
        var result = new List<ChatInfoPerUser>();

        await _lockerQueue.InvokeAsync(async () =>
        {
            foreach (var chat in chats)
            {
                var room = _chatRoomRepository.FirstOrDefualt(x => x.Id == chat.ChatName);
                if (room is not null)
                {
                    result.Add(new ChatInfoPerUser(room.RoomName, room.ChatType, room.Users.Count(), chat.IsOwner));
                }
            }
        });

        return result;
    }


    private async Task<ChatDto> GetRoomByNameOrIdCoreAsync(string nameOrId)
    {
        return _chatRoomRepository.FirstOrDefualt(x => x.Id == nameOrId);
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
        _chatRoomRepository.Remove(x => x.RoomName == nameOrId);
    }

    public async Task RemoveHistoryByRoomNameAsync(string roomName)
    {
        await _lockerQueue.InvokeAsync(async () =>
        {
            _chatRoomRepository.Update(x => x.Id == roomName, x => x.UserMessages.Clear());
            //var room = await _chatRoomRepository.GetOrDefaultAsync(x => x.Id == roomName);
            //if (room is not null)
            //{
            //    room.UserMessages.Clear();
            //}
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

                    var chatRoom = _chatRoomRepository.FirstOrDefualt(x => x.Id == chatName);
                    if (chatRoom is not null)
                    {

                        var amountMessages = chatRoom.Messages.Count() + 1;
                        var localPath = $"{amountMessages}{Path.GetExtension(imageName)}";


                        var filePath = Path.Combine(_env.WebRootPath, IChatService.ChatImages, chatName, localPath);
                        byte[] fileBytes = Convert.FromBase64String(textMessage);
                        //string base64String = Convert.ToBase64String(fileBytes);

                        try
                        {
                            FileHelper.SaveFile(fileBytes, filePath);
                        }
                        catch (Exception ex)
                        {

                        }

                        var imageFilePath = $"{IChatService.ChatImages}/{chatName}/{localPath}";
                        var senderinfo = new SenderInfo(SenderInfoType.Image, fromUser, textMessage, imageFilePath, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                        _chatRoomRepository.Update(x => x.RoomName == chatName, x => x.UserMessages.Add(senderinfo));


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
            var anyActiveUsers = false;
            var isUpdated = _chatRoomRepository.Update(x => x.Id == roomName, x =>
            {
                x.ActiveUsers.Remove(username);
                anyActiveUsers = x.ActiveUsers.Any();
            });


            if (!anyActiveUsers)
            {
                await RemoveRoomByNameOrIdCoreAsync(roomName);
            }
        });
    }

    public async Task<SenderInfo> JoinOrCreateRoom(string roomName, string userName, ChatType chatType, string description)
    {
        var result = default(SenderInfo);

        await _lockerQueue.InvokeAsync(async () =>
        {
            var room = await GetRoomByNameOrIdCoreAsync(roomName);
            var senderInfoType = SenderInfoType.Joined;

            if (room is null)
            {
                room = await CreateRoomCoreAsync(roomName, chatType, description);
                senderInfoType = SenderInfoType.Created;
            }

            result = new SenderInfo(senderInfoType, userName, null, null, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var isUpdated = _chatRoomRepository.Update(x => x.RoomName == roomName, x =>
            {
                x.ActiveUsers.Add(userName);
                x.UserMessages.Add(result);
            });


            //room.ActiveUsers.Add(userName);
            //result = await AddTextMessage(senderInfoType, roomName, userName, null, null);


            //var room = await GetRoomByNameOrIdCoreAsync(roomName);
            //if (room is not null)
            //{
            //    room.ActiveUsers.Add(userName);
            //    result = await AddTextMessage(SenderInfoType.Joined, roomName, userName, null, null);
            //}
            //else
            //{
            //    var createRoom = await CreateRoomCoreAsync(roomName, chatType, description);
            //    createRoom.ActiveUsers.Add(userName);
            //    result = await AddTextMessage(SenderInfoType.Created, roomName, userName, null, null);
            //}
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
                await CreateRoomCoreAsync(IChatService.Lobi, ChatType.Public, null);
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

    public async Task<IEnumerable<string>> UploadFilesAsync(UserDto user, string chatName, IEnumerable<IFormFile> documents)
    {

        await _chatValidationService.UploadDocumentsValidationAsync(user, chatName, documents);

        IEnumerable<string> response = null;

        var fileInfoes = await FileHelper.DissectAsync(documents);

        await _lockerQueue.InvokeAsync(async () =>
        {
            var chat = _chatRoomRepository.FirstOrDefualt(x => x.RoomName == chatName);
            if (chat is not null)
            {
                //await _chatRoomRepository.UpdateImagesAsync(chatName);
                var amountOfImages = 1;
                var template = string.Format(IChatService.ChatUploadedImagesTemplate, chatName);
                var wwwRootPath = Path.Combine(_env.WebRootPath, template);
                var files = new List<string>();
                foreach (var fileInfo in fileInfoes)
                {
                    var localFileame = $"{amountOfImages}{Path.GetExtension(fileInfo.FileName)}";
                    var filePath = Path.Combine(wwwRootPath, localFileame);
                    try
                    {
                        FileHelper.SaveFile(fileInfo.Content, filePath);
                    }
                    catch (Exception ex)
                    {

                    }

                    var webUrl = Path.Combine(template, localFileame);
                    files.Add(webUrl);
                }

                await _chatRoomRepository.UpdateImagesAsync(x => x.RoomName == chatName, files);
                response = files;
            }
        });

        return response;
    }


    //----------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------

    private async Task<SenderInfo> AddTextMessage(SenderInfoType senderInfoType, string chatName, string fromUser, string? textMessage, string? image)
    {

        var senderInfo = new SenderInfo(senderInfoType, fromUser, textMessage, image, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var isUpdated = _chatRoomRepository.Update(x => x.RoomName == chatName, x => x.UserMessages.Add(senderInfo));

        var result = isUpdated ? senderInfo : null;
        return result;
    }
}
