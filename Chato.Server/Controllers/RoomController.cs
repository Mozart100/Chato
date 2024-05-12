using Chato.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;


[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IUserService _userService;

    public RoomController(IRoomService roomService, IUserService userService)
    {
        this._roomService = roomService;
        this._userService = userService;
    }


    [HttpGet]
    [Route("")]
    public async Task<ActionResult<GetAllRoomResponse>> GetAllRooms()
    {
       var result  = await _roomService.GetAllRoomAsync();
        return Ok(new GetAllRoomResponse { Rooms = result });
    }


    [HttpGet]
    [Route("users")]
    public async Task<ActionResult<GetAllRoomResponse>> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(new GetAllUserResponse { Users = result.ToArray() });
    }



}
