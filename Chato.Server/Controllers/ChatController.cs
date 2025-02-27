using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

[Route("api/[controller]")]
public class ChatController : ChattoBaseController
{
    public const string Chat_Name = "chatName";
    public const string All_Chat_Route = "all";
    //public const string Chat_Route = "{chat}";
    public const string UploadChatImagesUrl = $"upload/{{{Chat_Name}}}";

    private readonly IChatService _roomService;
    private readonly IAssignmentService _assignmentService;

    public ChatController(IChatService roomService,
        IAssignmentService assignmentService)
    {
        this._roomService = roomService;
        this._assignmentService = assignmentService;
    }


    [HttpGet]
    [Route(All_Chat_Route)]
    public async Task<GetAllRoomResponse> GetAllRooms()
    {
        var result = await _roomService.GetAllRoomAsync();
        var response = new GetAllRoomResponse { Rooms = result};

        return response;
    }


    [HttpGet]
    [Route("{chatName}"), Authorize]
    public async Task<GetRoomResponse> GetRoom(string chatName)
    {
        var room = await _roomService.GetRoomByNameOrIdAsync(chatName);

        return new GetRoomResponse { Chat = room };
    }


    [HttpPost]
    [Route(UploadChatImagesUrl)]
    public async Task<UploadDocumentsResponse> Upload([FromRoute] string chatName, IEnumerable<IFormFile> documents)
    {
        var response = new UploadDocumentsResponse();
        var userName = User.Identity.Name;

        var files = await _assignmentService.UploadFilesToChatAsync(chatName, CurrentUser, documents);
        if (files.IsNullOrEmpty() == false)
        {
            response.Files.AddRange(files);
        }

        return response;
    }



}
