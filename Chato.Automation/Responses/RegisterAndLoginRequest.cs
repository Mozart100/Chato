using Chato.Server;

namespace Chato.Automation.Responses;

internal class RegisterAndLoginRequest :UserDto
{
}

public class RegisterResponse : User
{

}



internal class LoginRequest : RegisterAndLoginRequest
{

}