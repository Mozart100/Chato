﻿using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

public class ChatoResponseResult : ObjectResult 
{
    public ChatoResponseResult(object response)
        : base(response)
    {
    }
}

public class ChatoResponseWrapper
{

    public ChatoResponseWrapper(object response)
    {
        Response = response;
        IsSuccess = true;
    }

    public bool IsSuccess { get; }
    public object Response { get; }
}

public class ChatoController : ControllerBase
{
    public ObjectResult ChatoResponse(object response)
    {

        return new ChatoResponseResult(response);
    }
}


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
    [Route(All_Route)]
    public async Task<ActionResult<GetAllRoomResponse>> GetAllRooms()
    {
       var result  = await _roomService.GetAllRoomAsync();

        //var dtos = result.SafeSelect(x => new ChatRoomDto(x.RoomName, x.SenderInfo.SafeToArray(), x.Users.SafeToArray()));

        return Ok(new GetAllRoomResponse { Rooms = result.SafeToArray()});
    }


    [HttpGet]
    [Route("{roomName}")]
    public async Task<ActionResult<ChatRoomDto>> GetRoom(string roomName)
    {
        var room = await _roomService.GetRoomByNameOrIdAsync(roomName);
        return Ok(room);
    }


    [HttpGet] 
    [Route("users")]
    public async Task<ActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();

        return Ok(new GetAllUserResponse { Users = result.ToArray() });
    }



}
