using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly IUserService _UserService;

        public UserController(IMapper mapper,
            IUserService userService)
        {
            this._mapper = mapper;
            _UserService = userService;
        }


        [HttpGet, Authorize]
        public async Task<UserResponse> Get()
        {
            var result = default(UserResponse);

            var userName = User.Identity.Name;
            User user = await _UserService.GetUserByNameOrIdGetOrDefaultAsync(userName);
            if (user is not null)
            {
                var dto = _mapper.Map<UserDto>(user);
                result = new Chatto.Shared.UserResponse { User = dto };
            }

            return result;
        }
    }
}
