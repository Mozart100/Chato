using Chato.Server.Configuration;
using Chato.Server.Controllers;
using Chatto.Shared;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

public abstract class ChatoRawDataScenarioBase : ScenarioBase
{
    protected ChatoRawDataScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        HubUrl = $"{BaseUrl}/chat";
        AuthControllerUrl = $"{BaseUrl}/api/auth";
        UserControllerUrl = $"{BaseUrl}/api/user";


        RegisterAuthControllerUrl = $"{AuthControllerUrl}/{AuthController.RegistrationUrl}";
        UploadFilesUrl = $"{AuthControllerUrl}/{AuthController.UploadUrl}";
        DownloadFileUrl = $"{AuthControllerUrl}/{AuthController.DownloadUrl}";


        RoomsControllerUrl = $"{BaseUrl}/api/room";
        GetAllRoomsUrl = $"{RoomsControllerUrl}/{RoomController.All_Chat_Route}";
        GetAllUsersUrl = $"{RoomsControllerUrl}/users";


        var argument = "{0}";
        SpecificRoomTemplatesUrl = $"{RoomsControllerUrl}/{argument}";


        ConfigurationControllerUrl = $"{BaseUrl}/api/configuration";
        GetEvictionConfigurationUrl = $"{ConfigurationControllerUrl}/{ConfigurationController.EvictionUrl}";

    }

    protected string HubUrl { get; }
    protected string AuthControllerUrl { get; }
    protected string UserControllerUrl { get; }
    protected string RegisterAuthControllerUrl { get; }
    protected string DownloadFileUrl { get; }
    protected string UploadFilesUrl { get; }
    

    protected string RoomsControllerUrl { get; }
    protected string SpecificRoomTemplatesUrl { get; }
    protected string GetAllRoomsUrl { get; }
    protected string GetAllUsersUrl { get; }




    protected string ConfigurationControllerUrl { get; }
    protected string GetEvictionConfigurationUrl { get; }



    public async Task<CacheEvictionRoomConfigDto> GetEvictionConfigurationAsync()
    {
        var response =  await Get<ResponseWrapper<CacheEvictionRoomConfigDto>>(GetEvictionConfigurationUrl);
        return response.Body;
    }


}
