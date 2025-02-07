using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.Services;

public interface IAssignmentService
{

    Task<SenderInfo> JoinOrCreateRoom(string nameOrId, string roomName,ChatType chatType);
    Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string defaultRoom);
    Task LeaveGroupByConnectionIdAsync(string connectionId);
    Task LeaveGroupByUserNameOrIdAsync(string userNameOrId);
    Task CreateLobi();
}

public class AssignmentService : IAssignmentService
{

    private readonly IUserService _userService;
    private readonly IChatService _roomService;
    private readonly IAuthenticationService _authenticationService;

    public AssignmentService(IUserService userService,
        IChatService roomService,
        IAuthenticationService authenticationService)
    {
        this._userService = userService;
        this._roomService = roomService;
        this._authenticationService = authenticationService;
    }

    public string Enterence => IChatService.Lobi;

    public async Task LeaveGroupByConnectionIdAsync(string connectionId)
    {
        var user = await _userService.GetUserByConnectionId(connectionId);
        await LeaveUserFromRoom(user);
    }

    public async Task LeaveGroupByUserNameOrIdAsync(string userNameOrId)
    {
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(userNameOrId);
        if (user is not null)
        {
            await LeaveUserFromRoom(user);
            await _userService.RemoveUserByUserNameOrIdAsync(userNameOrId);
        }
    }

    private async Task LeaveUserFromRoom(User user)
    {
        if (user is not null)
        {
            foreach (var roomName in user.Chats.SafeToArray())
            {
                await _roomService.RemoveUserAndRoomFromRoom(roomName, user.UserName);
            }
        }
    }

    public async Task<SenderInfo> JoinOrCreateRoom(string nameOrId, string chatName, ChatType chatType)
    {
        var result =default( SenderInfo);

        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(nameOrId);
        if (user is not null)
        {
            await _userService.AssignRoomNameAsync(user.UserName, chatName);
           result =  await _roomService.JoinOrCreateRoom(chatName, user.UserName,chatType);
        }

        return result;
    }

    public async Task CreateLobi()
    {

        await _roomService.CreateLobi();
        
    }

    public async Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string roomName)
    {
        var token = await _authenticationService.RegisterAsync(request);

        await JoinOrCreateRoom(request.UserName, roomName,ChatType.Public);

        return token;
    }
}
