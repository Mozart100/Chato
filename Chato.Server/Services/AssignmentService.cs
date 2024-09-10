using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.Services;

public interface IAssignmentService
{
    public const string Lobi = "lobi";

    Task JoinGroup(string nameOrId, string roomName);
    Task JoinLobi(string nameOrId);
    Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string defaultRoom);
    Task RemoveUserByConnectionIdAsync(string connectionId);
    Task RemoveUserByUserNameOrIdAsync(string userNameOrId);
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

    public string Enterence => IAssignmentService.Lobi;

    public async Task RemoveUserByConnectionIdAsync(string connectionId)
    {
        var user = await _userService.GetUserByConnectionId(connectionId);
        await RemoveUserFromRoom(user);
    }

    public async Task RemoveUserByUserNameOrIdAsync(string userNameOrId)
    {
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(userNameOrId);
        if (user is not null)
        {
            await RemoveUserFromRoom(user);
            await _userService.RemoveUserByUserNameOrIdAsync(userNameOrId);
        }
    }

    private async Task RemoveUserFromRoom(User user)
    {
        if (user is not null)
        {
            foreach (var roomName in user.Chats.SafeToArray())
            {
                await _roomService.RemoveUserAndRoomFromRoom(roomName, user.UserName);
            }
        }
    }

    public async Task JoinGroup(string nameOrId, string chatName)
    {
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(nameOrId);
        if (user is not null)
        {
            await _userService.AssignRoomNameAsync(user.UserName, chatName);
            await _roomService.JoinOrCreateRoom(chatName, user.UserName);
        }
    }

    public async Task JoinLobi(string nameOrId)
    {
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(nameOrId);
        if (user is not null)
        {
            await _userService.AssignRoomNameAsync(user.UserName, Enterence);
            await _roomService.JoinOrCreateRoom(Enterence, user.UserName);
        }
    }

    public async Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string roomName)
    {
        var token = await _authenticationService.RegisterAsync(request);

        await JoinGroup(request.UserName, roomName);

        return token;
    }
}
