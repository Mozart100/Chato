using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chatto.Shared;
using System.Security.Claims;

namespace Chato.Server.Services;


public interface IUserService
{
    Task AssignConnectionId(string userName, string connectionId);
    Task AssignRoomNameAsync(string userNameOrId, string roomName);
    Task<IEnumerable<User>> GetAllUsersAsync();
    string GetMyName();
    Task<User> GetUserByConnectionId(string connectionId);
    Task<User> GetUserByNameOrIdGetOrDefaultAsync(string nameOrId);
    Task RegisterAsync(string username, string description, string gender, int age);
    //Task RegisterAsync(string username, byte[] passwordHash, string description, string gender, int age);
    Task<bool> RemoveUserByUserNameOrIdAsync(string userNameOrId);
    Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<byte[]> files);
}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IDelegateQueue _delegateQueue;

    public UserService(IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IDelegateQueue delegateQueue)
    {
        _httpContextAccessor = httpContextAccessor;
        this._userRepository = userRepository;
        this._delegateQueue = delegateQueue;
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

    public async Task RegisterAsync(string username,  string description, string gender, int age)
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

    public async Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<byte[]> files)
    {
        var response = new UploadDocumentsResponse();

        await _delegateQueue.InvokeAsync(async () =>
        {
            await _userRepository.UpdateAsync(user => user.UserName == userName, user =>
            {
                response.Document1 = (user.Document1 = files.ElementAtOrDefault(0)) is not null;
                response.Document2 = (user.Document2 = files.ElementAtOrDefault(1)) is not null;
                response.Document3 = (user.Document3 = files.ElementAtOrDefault(2)) is not null;
                response.Document4 = (user.Document4 = files.ElementAtOrDefault(3)) is not null;
                response.Document5 = (user.Document5 = files.ElementAtOrDefault(4)) is not null;
            });
        });

        return response;
    }
}
