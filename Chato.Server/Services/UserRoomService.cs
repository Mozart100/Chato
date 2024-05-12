using Chato.Server.DataAccess.Models;
using Chato.Server.DataAccess.Repository;

namespace Chato.Server.Services;

public interface IUserRoomService
{
    Task JoinGroupByConnectionId(string nameOrId, string roomName);
    Task RemoveUserByConnectionIdAsync(string connectionId);
    Task RemoveUserByUserNameOrIdAsync(string userNameOrId);
}

public class UserRoomService : IUserRoomService
{
    private readonly IUserService _userService;
    private readonly IRoomService _roomService;

    public UserRoomService(IUserService userService, IRoomService roomService)
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
                //ptr.Users.Add(user.UserName); 
            }
        }
    }
}
