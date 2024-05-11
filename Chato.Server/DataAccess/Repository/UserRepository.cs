using Chato.Server.DataAccess.Models;

namespace Chato.Server.DataAccess.Repository;


public interface IUserRepository : IRepositoryBase<UserDb>
{
    Task AssignConectionnId( string userName ,  string conectionnId);
}


public class UserRepository : RepositoryBase<UserDb>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }

    public async Task AssignConectionnId(string userName, string conectionnId)
    {
        var user  = await GetAsync(x => x.UserName == userName);
        user.ConnectionId = conectionnId;
    }
}