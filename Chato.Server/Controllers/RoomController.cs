using Chato.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;


[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService roomService;

    public RoomController(IRoomService roomService)
    {
        this.roomService = roomService;
    }


    [HttpGet]
    [Route("")]
    public async Task<ActionResult<GetAllRoomResponse>> GetAllRooms()
    {
       var result  = await roomService.GetAllRoomAsync();
        return Ok(new GetAllRoomResponse { Rooms = result });
    }
}
