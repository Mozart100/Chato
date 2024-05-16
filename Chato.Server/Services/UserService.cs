using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
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

    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        this._userRepository = userRepository;
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
        await _userRepository.InsertAsync(new UserDb { Id = username, PasswordHash = passwordHash});

    }


    public async Task AssignConectionnId(string userName, string connectionId)
    {
        await _userRepository.AssignConnectionId(userName, connectionId);

    }

    public async Task<UserDb> GetUserByConnectionId(string connectionId)
    {
        var user = await _userRepository.GetOrDefaultAsync(x => x.ConnectionId == connectionId);
        return user;
    }

    public async Task<bool> RemoveUserByUserNameOrIdAsync(string userNameOrId)
    {
        var result = await _userRepository.RemoveAsync(x => x.UserName == userNameOrId);
        return result;
    }

    public async Task<UserDb> GetUserByNameOrIdAsync(string nameOrId)
    {
        return await _userRepository.GetOrDefaultAsync(x => x.UserName == nameOrId);
    }

    public async Task<IEnumerable<UserDb>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

  
}
