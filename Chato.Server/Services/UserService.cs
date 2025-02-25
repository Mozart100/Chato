﻿using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services.Validations;
using Chatto.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Chato.Server.Services;


public interface IUserService
{
    public const string UserChatImage = "UserImages";

    Task AssignConnectionId(string userName, string connectionId);
    Task AssignRoomNameAsync(string userNameOrId, string roomName, ChatType chatType, bool isOwner);
    Task<IEnumerable<User>> GetAllUsersAsync(Func<User, bool> predicate);
    string GetMyName();
    Task<User> GetUserByConnectionId(string connectionId);
    Task<User> GetUserByNameOrIdGetOrDefaultAsync(string nameOrId);
    Task RegisterAsync(string username, string description, string gender, int age);
    Task<bool> RemoveUserByUserNameOrIdAsync(string userNameOrId);
    //Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<UserFileInfo> files);
    public Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<IFormFile> documents);


    Task<IEnumerable<string>> DownloadFilesAsync(string userName);
    Task<IEnumerable<ParticipantInChat>> GetUserChatsAsync(string userNameOrId);

}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserValidationService _userValidationService;
    private readonly IUserRepository _userRepository;
    private readonly ILockerDelegateQueue _delegateQueue;
    private readonly IWebHostEnvironment _env;

    public UserService(IHttpContextAccessor httpContextAccessor,
        IUserValidationService userValidationService,
        IUserRepository userRepository,
        ILockerDelegateQueue delegateQueue,
        IWebHostEnvironment env)
    {
        _httpContextAccessor = httpContextAccessor;
        this._userValidationService = userValidationService;
        this._userRepository = userRepository;
        this._delegateQueue = delegateQueue;
        this._env = env;
    }

    public string GetMyName()
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
        return result;
    }

    public async Task RegisterAsync(string username, string description, string gender, int age)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.InsertAsync(new User { Id = username, Description = description, Gender = gender, Age = age }));
    }


    public async Task AssignRoomNameAsync(string userNameOrId, string roomName, ChatType chatType, bool isOwner)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.AddRoomToUser(userNameOrId, roomName, chatType,isOwner));

    }

    public async Task AssignConnectionId(string userName, string connectionId)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.AssignConnectionId(userName, connectionId));
    }

    public async Task<User> GetUserByConnectionId(string connectionId)
    {
        var result = default(User);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.GetOrDefaultAsync(x => x.ConnectionId == connectionId);
        });
        return result;
    }

    public async Task<bool> RemoveUserByUserNameOrIdAsync(string userNameOrId)
    {
        bool result = false;

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.RemoveAsync(x => x.UserName == userNameOrId);
        });
        return result;
    }

    public async Task<User> GetUserByNameOrIdGetOrDefaultAsync(string nameOrId)
    {
        var result = default(User);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.GetOrDefaultAsync(x => x.UserName == nameOrId);
        });
        return result;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(Func<User, bool> predicate)
    {
        var result = Enumerable.Empty<User>();

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.GetAllAsync(predicate);
        });

        return result;
    }

    public async Task<IEnumerable<ParticipantInChat>> GetUserChatsAsync(string userNameOrId)
    {
        var result = Enumerable.Empty<ParticipantInChat>();

        await _delegateQueue.InvokeAsync(async () =>
        {
            var user = await _userRepository.GetOrDefaultAsync(u => u.UserName == userNameOrId);
            if (user is not null)
            {
                result = user.Chats.ToArray();
            }
        });

        return result;
    }

    public async Task<IEnumerable<string>> DownloadFilesAsync(string userName)
    {
        IEnumerable<string> files = null;

        await _delegateQueue.InvokeAsync(async () =>
        {
            files = await _userRepository.DownloadFiles(userName);
        });

        return files;
    }

    public async Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<IFormFile> documents)
    {
        await _userValidationService.UploadDocumentsValidationAsync(documents);
        
        var data = await FileHelper.DissectAsync(documents);
        var files = await UploadFilesAsync(userName, data);

        return files;
    }


    private async Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<(string FileName, byte[] Content)> files)
    {
        var response = new UploadDocumentsResponse();

        await _delegateQueue.InvokeAsync(async () =>
        {
            await _userRepository.UpdateAsync(user => user.UserName == userName, user =>
            {
                var amountOfImages = 1;
                var wwwRootPath = Path.Combine(_env.WebRootPath, IUserService.UserChatImage, userName);
                if (Directory.Exists(wwwRootPath) == false)
                {
                    Directory.CreateDirectory(wwwRootPath);
                }

                foreach (var file in files)
                {
                    var localFileame = $"{amountOfImages}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(wwwRootPath, localFileame);
                    try
                    {
                        File.WriteAllBytes(filePath, file.Content);
                    }
                    catch (Exception ex)
                    {

                    }

                    var webPath = $"{IUserService.UserChatImage}/{userName}/{localFileame}";
                    user.FileSegment.Add(webPath);
                    response.Files.Add(webPath);

                    amountOfImages++;
                }
            });
        });

        return response;
    }


}
