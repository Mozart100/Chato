using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.DataAccess.Repository;


public interface IUserRepository : IRepositoryBase<UserDb, User>
{
    Task<bool> AddRoomToUser(string userNameOrId, string roomName, ChatType chatType, bool isOwner);
    Task AssignConnectionId(string userName, string connectionId);
    Task<IEnumerable<string>> DownloadFiles(string userName);

}


public class UserRepository : AutoRepositoryBase<UserDb, User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger, IMapper mapper)
        : base(mapper)
    {
        _logger = logger;
    }

    public async Task<bool> AddRoomToUser(string userNameOrId, string roomName, ChatType chatType, bool isOwner)
    {
        var result = false;
        await UpdateAsync(u => u.UserName == userNameOrId, user =>
        {
            var rooms = user.Chats.SafeToHashSet();
            result = rooms.Add(new ParticipantInChat(roomName, chatType, isOwner));

            if (result)
            {
                user.Chats = rooms.ToArray();
            }
        });

        return result;
    }

    public virtual async Task AssignConnectionId(string userName, string conectionId)
    {
        await UpdateAsync(u => u.UserName == userName, user => user.ConnectionId = conectionId);
    }

    public async Task<IEnumerable<string>> DownloadFiles(string userName)
    {
        IEnumerable<string> result = [];

        var model = Models.FirstOrDefault(x => x.UserName == userName);

        if (model is not null)
        {
            result = model.FileSegment.GetImages();
        }

        return result;
    }
}
