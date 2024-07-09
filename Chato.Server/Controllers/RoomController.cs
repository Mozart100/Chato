using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    public const string All_Route = "all";
    public const string Room_Route = "{room}";

    private readonly IRoomService _roomService;
    private readonly IUserService _userService;

    public RoomController(IRoomService roomService, IUserService userService)
    {
        this._roomService = roomService;
        this._userService = userService;
    }


    [HttpGet]
    [Route(All_Route), Authorize]
    public async Task<GetAllRoomResponse> GetAllRooms()
    {
        var result = await _roomService.GetAllRoomAsync();
        return new GetAllRoomResponse { Rooms = result.SafeToArray() };
    }


    [HttpGet]
    [Route("{roomName}"),Authorize]
    public async Task<GetRoomResponse> GetRoom(string roomName)
    {
        var room = await _roomService.GetRoomByNameOrIdAsync(roomName);

        return new GetRoomResponse { Room = room };
    }


    [HttpGet]
    [Route("users"), Authorize]
    public async Task<GetAllUserResponse> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();

        return new GetAllUserResponse { Users = result.ToArray() };
    }



}
