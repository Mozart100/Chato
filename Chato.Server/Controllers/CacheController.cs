using Chato.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

//public class ChatoResponseResult : ObjectResult
//{
//    public ChatoResponseResult(object response)
//        : base(response)
//    {
//    }
//}

//public class ChatoResponseWrapper
//{

//    public ChatoResponseWrapper(object response)
//    {
//        Response = response;
//        IsSuccess = true;
//    }

//    public bool IsSuccess { get; }
//    public object Response { get; }
//}

//public class ChatoController : ControllerBase
//{
//    public ObjectResult ChatoResponse(object response)
//    {

//        return new ChatoResponseResult(response);
//    }
//}

[Route("api/[controller]")]
[ApiController]
public class CacheController : ControllerBase
{
    public const string All_Route = "all";
    public const string Room_Route = "{room}";

    private readonly IRoomService _roomService;
    private readonly IUserService _userService;

    public CacheController(IRoomService roomService, IUserService userService)
    {
        this._roomService = roomService;
        this._userService = userService;
    }

}
