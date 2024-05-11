using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;
using System.Security.Claims;

namespace Chato.Server.Services;


public interface IUserService
{
    string GetMyName();
    Task LoginAsync(string username);
}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository userRepository;

    public UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
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

    public async Task LoginAsync(string username)
    {
        await userRepository.InsertAsync( new UserDb { Id = username });  
    }
}
