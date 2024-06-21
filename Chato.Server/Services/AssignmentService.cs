using Chato.Server.DataAccess.Models;
using Chato.Server.Models.Dtos;

namespace Chato.Server.Services;

public interface IAssignmentService
{
    Task JoinGroupByConnectionId(string nameOrId, string roomName);
    Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string defaultRoom);
    Task RemoveUserByConnectionIdAsync(string connectionId);
    Task RemoveUserByUserNameOrIdAsync(string userNameOrId);
}

public class AssignmentService : IAssignmentService
{
    private readonly IUserService _userService;
    private readonly IRoomService _roomService;
    private readonly IAuthenticationService _authenticationService;

    public AssignmentService(IUserService userService,
        IRoomService roomService,
        IAuthenticationService authenticationService)
    {
        this._userService = userService;
        this._roomService = roomService;
        this._authenticationService = authenticationService;
    }

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
            foreach (var roomName in user.Rooms.ToArray())
            {
                await _roomService.RemoveUserAndRoomFromRoom(roomName, user.UserName);
            }
        }
    }

    public async Task JoinGroupByConnectionId(string nameOrId, string roomName)
    {
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(nameOrId);
        if (user is not null)
        {
            await _userService.AssignRoomNameAsync(user.UserName, roomName);
            await _roomService.JoinOrCreateRoom(roomName, user.UserName);
        }
    }

    public async Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string roomName)
    {
        var token = await _authenticationService.RegisterAsync(request);

        await JoinGroupByConnectionId(request.UserName, roomName);

        return token;
    }
}
