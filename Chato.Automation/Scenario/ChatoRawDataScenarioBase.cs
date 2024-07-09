using Chato.Server.Controllers;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

public abstract class ChatoRawDataScenarioBase : ScenarioBase
{
    protected ChatoRawDataScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        HubUrl = $"{BaseUrl}/chat";
        AuthControllerUrl = $"{BaseUrl}/api/auth";
        RegisterAuthControllerUrl = $"{AuthControllerUrl}/register";
        UploadFilesUrl = $"{AuthControllerUrl}/upload";


        RoomsControllerUrl = $"{BaseUrl}/api/room";
        GetAllRoomsUrl = $"{RoomsControllerUrl}/{RoomController.All_Rooms_Route}";
        GetAllUsersUrl = $"{RoomsControllerUrl}/users";


        var argument = "{0}";
        SpecificRoomTemplatesUrl = $"{RoomsControllerUrl}/{argument}";
    }

    protected string HubUrl { get; }
    protected string AuthControllerUrl { get; }
    protected string RegisterAuthControllerUrl { get; }
    protected string UploadFilesUrl { get; }


    protected string RoomsControllerUrl { get; }
    protected string SpecificRoomTemplatesUrl { get; }
    protected string GetAllRoomsUrl { get; }
    protected string GetAllUsersUrl { get; }

}
