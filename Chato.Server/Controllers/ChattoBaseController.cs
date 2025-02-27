using Chatto.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

[ApiController]
public abstract class ChattoBaseController : ControllerBase
{
    protected UserDto CurrentUser
    {
        get
        {
            if (HttpContext.Items.TryGetValue(Chato.Server.DataAccess.Models.User.User_Key, out var userObj) && userObj is UserDto userDto)
            {
                return userDto;
            }

            throw new Exception("xxx");
        }
    }
}
