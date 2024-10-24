using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;

namespace Chato.Server.DataAccess.Repository;


public interface IUserRepository : IRepositoryBase<UserDb, User>
{
    Task AddRoomToUser(string userNameOrId, string roomName);
    Task AssignConnectionId(string userName, string connectionId);
    Task<IEnumerable<UserFileInfo>> DownloadFiles(string userName);

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
            var rooms = user.Chats.SafeToHashSet();
            rooms.Add(roomName);
            user.Chats = rooms.ToArray();
        });

    }

    public virtual async Task AssignConnectionId(string userName, string conectionId)
    {
        await UpdateAsync(u => u.UserName == userName, user => user.ConnectionId = conectionId);
    }

    public async Task<IEnumerable<UserFileInfo>> DownloadFiles(string userName)
    {
        UserFileInfo[] result = [];

        var model = Models.FirstOrDefault(x=>x.UserName == userName);

        if(model is not null)
        {
           result=model.Files.ToArray();
        }

        return result;
    }
}
