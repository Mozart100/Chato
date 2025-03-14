﻿using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers
{

    [Route("api/[controller]")]
    public class UserController : ChattoBaseController
    {
        public const string All_Users_Route = "all";
        public const string Chats_Per_User_Route = "chatperuser";
        public const string UserUploadUrl = "upload";

        private readonly IMapper _mapper;
        public readonly IUserService _userService;
        private readonly IChatService _chatService;

        public UserController(IMapper mapper,
            IUserService userService,
            IChatService chatService)
        {
            this._mapper = mapper;
            _userService = userService;
            this._chatService = chatService;
        }


        [HttpGet, Authorize]
        public async Task<UserResponse> GetSelf()
        {
            //var result = default(UserResponse);

            //var userName = User.Identity.Name;
            //User user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(userName);
            //if (user is not null)
            //{
            //    var dto = _mapper.Map<UserDto>(user);
            //    result = new Chatto.Shared.UserResponse { User = dto };
            //}


            return new UserResponse { User = CurrentUser };
        }

        [HttpGet]
        [Route(All_Users_Route)]
        public async Task<GetAllUserResponse> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync(x => true);

            return new GetAllUserResponse { Users = result.ToArray() };
        }

        [HttpGet]
        [Route(Chats_Per_User_Route)]
        public async Task<AllChatsPerUserResponse> GetChatsPerUser()
        {
            var userName = User.Identity.Name;

            var chatInfoes = await _userService.GetUserChatsAsync(userName);
            var data = await _chatService.GetChatInfoPerChatName(chatInfoes);
            var result = new AllChatsPerUserResponse(data);

            return result;
        }


        [Route(UserUploadUrl)]
        [HttpPost, Authorize]
        public async Task<ActionResult<UploadDocumentsResponse>> Upload(IEnumerable<IFormFile> documents)
        {
            var userName = User.Identity.Name;
            var response = await _userService.UploadFilesAsync(userName, documents);

            return Ok(response);
        }
    }
}
