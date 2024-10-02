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
        var result = new List<UserFileInfo>();

        var model = Models.FirstOrDefault(x=>x.UserName == userName);

        if(model is not null)
        {
            if(model.Document1 is not null)
            {
                result.Add(model.Document1);
            }

            if (model.Document2 is not null)
            {
                result.Add(model.Document2);
            }

            if (model.Document3 is not null)
            {
                result.Add(model.Document3);
            }

            if (model.Document4 is not null)
            {
                result.Add(model.Document4);
            }

            if (model.Document5 is not null)
            {
                result.Add(model.Document5);
            }
        }

        return result;
    }
}
