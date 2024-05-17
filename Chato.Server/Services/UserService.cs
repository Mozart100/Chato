using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Chato.Server.Services;


public interface IUserService
{
    Task AssignConectionnId(string userName, string connectionId);
    Task<IEnumerable<UserDb>> GetAllUsersAsync();
    string GetMyName();
    Task<UserDb> GetUserByConnectionId(string connectionId);
    Task<UserDb> GetUserByNameOrIdAsync(string nameOrId);
    Task RegisterAsync(string username, byte[] passwordHash);
    Task<bool> RemoveUserByUserNameOrIdAsync(string userNameOrId);
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

    public async Task RegisterAsync(string username, byte[] passwordHash)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.InsertAsync(new UserDb { Id = username, PasswordHash = passwordHash }));
    }


    public async Task AssignConectionnId(string userName, string connectionId)
    {
        await _delegateQueue.InvokeAsync(async () => await _userRepository.AssignConnectionId(userName, connectionId));
    }

    public async Task<UserDb> GetUserByConnectionId(string connectionId)
    {
        var result = default(UserDb);

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

    public async Task<UserDb> GetUserByNameOrIdAsync(string nameOrId)
    {
        var result = default(UserDb);

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.GetOrDefaultAsync(x => x.UserName == nameOrId);
        });
        return result;
    }

    public async Task<IEnumerable<UserDb>> GetAllUsersAsync()
    {
        var result = Enumerable.Empty<UserDb>();

        await _delegateQueue.InvokeAsync(async () =>
        {
            result = await _userRepository.GetAllAsync();
        });

        return result;
    }

}
