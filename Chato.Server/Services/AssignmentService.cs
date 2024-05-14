using Chato.Server.DataAccess.Models;

namespace Chato.Server.Services;

public interface IAssignmentService
{
    Task JoinGroupByConnectionId(string nameOrId, string roomName);
    //Task RegisterToDefaultRoomAsync(string username, byte[] passwordHash);
    Task RemoveUserByConnectionIdAsync(string connectionId);
    Task RemoveUserByUserNameOrIdAsync(string userNameOrId);
}

public class AssignmentService : IAssignmentService
{
    private readonly IUserService _userService;
    private readonly IRoomService _roomService;

    public AssignmentService(IUserService userService, IRoomService roomService)
    {
        this._userService = userService;
        this._roomService = roomService;
    }

    public async Task RemoveUserByConnectionIdAsync(string connectionId)
    {
        var user = await _userService.GetUserByConnectionId(connectionId);
        await RemoveUserFromRoom(user);
    }

    public async Task RemoveUserByUserNameOrIdAsync(string userNameOrId)
    {
        var user = await _userService.GetUserByNameOrIdAsync(userNameOrId);
        if (user is not null)
        {
            await RemoveUserFromRoom(user);
            await _userService.RemoveUserByUserNameOrIdAsync(userNameOrId);
        }
    }

    private async Task RemoveUserFromRoom(UserDb user)
    {
        if (user is not null)
        {
            foreach (var roomName in user.Rooms.ToArray())
            {
                var room = await _roomService.GetRoomByNameOrIdAsync(roomName);
                if (room is not null)
                {
                    room.Users.Remove(user.UserName);

                    if (room.Users.Any() == false)
                    {
                        await _roomService.RemoveRoomByNameOrIdAsync(roomName);
                    }
                }
            }
        }
    }

    public async Task JoinGroupByConnectionId(string nameOrId, string roomName)
    {
        var user = await _userService.GetUserByNameOrIdAsync(nameOrId);
        if (user is not null)
        {
            user.Rooms.Add(roomName);
            var room = await _roomService.GetRoomByNameOrIdAsync(roomName);
            if (room is not null)
            {
                room.Users.Add(user.UserName);
            }
            else
            {
                var ptr = await _roomService.CreateRoomAsync(roomName);
                await _roomService.AddUserAsync(roomName, user.UserName);
            }
        }
    }

    public Task RegisterToDefaultRoomAsync(string username, byte[] passwordHash)
    {
        throw new NotImplementedException();
    }
}
