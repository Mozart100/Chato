namespace Chatto.Shared;


public class RegistrationRequest
{
    public string UserName { get; set; } = string.Empty;
    //public string Password { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; } = string.Empty;
}

public class RegistrationResponse
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    //public DateTime TokenCreated { get; set; }
    //public DateTime TokenExpires { get; set; }

    public int Age { get; set; }
    public string Description { get; set; }
    public string Gender { get; set; }
}

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Expires { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
}
