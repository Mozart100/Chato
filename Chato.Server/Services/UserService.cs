﻿using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chatto.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Chato.Server.Services;


public interface IUserService
{
    public const string UserChatImage = "ChattoUserImages";

    Task AssignConnectionId(string userName, string connectionId);
    Task AssignRoomNameAsync(string userNameOrId, string roomName);
    Task<IEnumerable<User>> GetAllUsersAsync();
    string GetMyName();
    Task<User> GetUserByConnectionId(string connectionId);
    Task<User> GetUserByNameOrIdGetOrDefaultAsync(string nameOrId);
    Task RegisterAsync(string username, string description, string gender, int age);
    Task<bool> RemoveUserByUserNameOrIdAsync(string userNameOrId);
    Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<UserFileInfo> files);
    Task<IEnumerable<UserFileInfo>> DownloadFilesAsync(string userName);

}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ILockerDelegateQueue _delegateQueue;
    private readonly IWebHostEnvironment _env;

    public UserService(IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        ILockerDelegateQueue delegateQueue,
        IWebHostEnvironment env)
    {
        _httpContextAccessor = httpContextAccessor;
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
        await _delegateQueue.InvokeAsync(async () => await _userRepository.InsertAsync(new UserDb { Id = username, Description = description, Gender = gender, Age = age }));
    }


    public async Task AssignRoomNameAsync(string userNameOrId, string roomName)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.AddRoomToUser(userNameOrId, roomName));

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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var result = Enumerable.Empty<User>();

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.GetAllAsync();
        });

        return result;
    }

    public async Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<UserFileInfo> files)
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

                    user.Files.Add(new UserFileInfo($"{IUserService.UserChatImage}/{userName}/{localFileame}", file.Content));
                    response.Files.Add(localFileame);
                   
                    amountOfImages++;
                }
            });
        });

        return response;
    }



    public async Task<IEnumerable<UserFileInfo>> DownloadFilesAsync(string userName)
    {
        IEnumerable<UserFileInfo> files = null;

        await _delegateQueue.InvokeAsync(async () =>
        {
            files = await _userRepository.DownloadFiles(userName);
        });

        return files;
    }
}
