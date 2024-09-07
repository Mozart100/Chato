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
    public const string All_Chat_Route = "all";
    public const string Chat_Route = "{room}";

    private readonly IChatService _roomService;
    private readonly IUserService _userService;

    public RoomController(IChatService roomService, IUserService userService)
    {
        this._roomService = roomService;
        this._userService = userService;
    }


    [HttpGet]
    [Route(All_Chat_Route)]
    public async Task<GetAllRoomResponse> GetAllRooms()
    {
        var result = await _roomService.GetAllRoomAsync();
        var response =  new GetAllRoomResponse { Rooms = result.SafeToArray() };
    
          return response;
    }


    [HttpGet]
    [Route("{roomName}"),Authorize]
    public async Task<GetRoomResponse> GetRoom(string roomName)
    {
        var room = await _roomService.GetRoomByNameOrIdAsync(roomName);

        return new GetRoomResponse { Room = room };
    }


    [HttpGet]
    [Route("users")]
    public async Task<GetAllUserResponse> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();

        return new GetAllUserResponse { Users = result.ToArray() };
    }



}
