﻿namespace Chato.Server.Models.Dtos;


public class UserRequest
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public class UserResponse
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}

public class RegisterResponse
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
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