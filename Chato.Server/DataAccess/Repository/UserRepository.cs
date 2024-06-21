using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;

namespace Chato.Server.DataAccess.Repository;


public interface IUserRepository : IRepositoryBase<UserDb, User>
{
    Task AddRoomToUser(string userNameOrId, string roomName);
    Task AssignConnectionId(string userName, string connectionId);
}


public class UserRepository : AutoRepositoryBase<UserDb, User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger, IMapper mapper)
        : base(mapper)
    {
        _logger = logger;
    }

    public async Task AddRoomToUser(string userNameOrId, string roomName)
    {
        await UpdateAsync(u => u.UserName == userNameOrId, user =>
        {
            var rooms = user.Rooms.SafeToList();
            rooms.Add(roomName);
            user.Rooms = rooms.ToArray();
        });

    }

    public virtual async Task AssignConnectionId(string userName, string conectionId)
    {
        await UpdateAsync(u => u.UserName == userName, user => user.ConnectionId = conectionId);
        //var user = await GetAsync(x => x.UserName == userName);
        //user.ConnectionId = conectionId;
    }
}
