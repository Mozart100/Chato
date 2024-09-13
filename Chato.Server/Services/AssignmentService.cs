using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chatto.Shared;

namespace Chato.Server.Services;

public interface IAssignmentService
{


    Task JoinOrCreateRoom(string nameOrId, string roomName);
    Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string defaultRoom);
    Task RemoveUserByConnectionIdAsync(string connectionId);
    Task RemoveUserByUserNameOrIdAsync(string userNameOrId);
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

    public async Task JoinOrCreateRoom(string nameOrId, string chatName)
    {
        var user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(nameOrId);
        if (user is not null)
        {
            await _userService.AssignRoomNameAsync(user.UserName, chatName);
            await _roomService.JoinOrCreateRoom(chatName, user.UserName);
        }
    }

    public async Task CreateLobi()
    {

        await _roomService.CreateLobi();
        
    }

    public async Task<string> RegisterUserAndAssignToRoom(RegistrationRequest request, string roomName)
    {
        var token = await _authenticationService.RegisterAsync(request);

        await JoinOrCreateRoom(request.UserName, roomName);

        return token;
    }
}
