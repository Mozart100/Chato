using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    public const string Chat_Name = "chatName";
    public const string All_Chat_Route = "all";
    public const string Chat_Route = "{chat}";
    public const string UploadChatImagesUrl = $"upload/{{{Chat_Name}}}";

    //[Route(UploadChatImagesUrl)]
    //[HttpPost, Authorize]

    private readonly IChatService _roomService;

    public ChatController(IChatService roomService)
    {
        this._roomService = roomService;
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
    [Route("{chatName}"),Authorize]
    public async Task<GetRoomResponse> GetRoom(string chatName)
    {
        var room = await _roomService.GetRoomByNameOrIdAsync(chatName);

        return new GetRoomResponse { Chat = room };
    }


    [HttpPost]
    [Route(UploadChatImagesUrl)]
    public async Task<UploadDocumentsResponse> Upload([FromRoute] string chatName, IEnumerable<IFormFile> documents)
    {
        var files = await _roomService.UploadFilesAsync(chatName, documents);
        var response = new UploadDocumentsResponse();
        response.Files.AddRange(files);
        return response;
    }



}
