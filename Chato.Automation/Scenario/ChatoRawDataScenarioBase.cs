﻿using Chato.Server.Configuration;
using Chato.Server.Controllers;
using Chato.Server.Hubs;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

public abstract class ChatoRawDataScenarioBase : ScenarioBase
{
    protected ChatoRawDataScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        HubUrl = $"{BaseUrl}{ChattoHub.HubMapUrl}";
        AuthControllerUrl = $"{BaseUrl}/api/auth";
        GetRawWwwrootUrl = $"{BaseUrl}";
        GetChattoImagesBaseUrl = $"{BaseUrl}/{IChatService.ChatImages}";


        RegisterAuthControllerUrl = $"{AuthControllerUrl}/{AuthController.RegistrationUrl}";
        UploadFilesUrl = $"{AuthControllerUrl}/{AuthController.UploadUrl}";
        DownloadFileUrl = $"{AuthControllerUrl}/{AuthController.DownloadUrl}";


        ChatsControllerUrl = $"{BaseUrl}/api/chat";
        GetAllRoomsUrl = $"{ChatsControllerUrl}/{ChatController.All_Chat_Route}";


        ///User
        UserControllerUrl = $"{BaseUrl}/api/user";
        GetAllUsersUrl = $"{UserControllerUrl}/all";


        var argument = "{0}";
        SpecificRoomTemplatesUrl = $"{ChatsControllerUrl}/{argument}";


        ConfigurationControllerUrl = $"{BaseUrl}/api/configuration";
        GetEvictionConfigurationUrl = $"{ConfigurationControllerUrl}/{ConfigurationController.EvictionUrl}";

    }

    protected string HubUrl { get; }
    protected string AuthControllerUrl { get; }
    protected string RegisterAuthControllerUrl { get; }
    protected string DownloadFileUrl { get; }
    protected string UploadFilesUrl { get; }


    protected string ChatsControllerUrl { get; }
    protected string SpecificRoomTemplatesUrl { get; }
    protected string GetAllRoomsUrl { get; }



    protected string UserControllerUrl { get; }
    protected string GetAllUsersUrl { get; }


    protected string ConfigurationControllerUrl { get; }
    protected string GetEvictionConfigurationUrl { get; }


    protected string GetRawWwwrootUrl { get; }
    protected string GetChattoImagesBaseUrl { get; }


    public string ImagePathCOmbine(string relativePath) => $"{GetRawWwwrootUrl}/{relativePath}";


    public async Task<CacheEvictionRoomConfigDto> GetEvictionConfigurationAsync()
    {
        var response = await Get<ResponseWrapper<CacheEvictionRoomConfigDto>>(GetEvictionConfigurationUrl);
        return response.Body;
    }


}
