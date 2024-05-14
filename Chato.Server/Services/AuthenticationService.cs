﻿using Chato.Server.Controllers;
using Chato.Server.Infrastracture;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Chato.Server.Services;

public interface IAuthenticationService
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    string CreateToken(string user);
    Task<string> RegisterAsync(string userName, string password);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfig _config;
    private readonly IUserService _userService;

    public AuthenticationService(IOptions<AuthenticationConfig> config,
        IUserService userService)
    {
        _config = config.Value;

        this._userService = userService;
    }


    public async Task<string>  RegisterAsync(string  userName, string password)
    {
        var token = CreateToken(userName);

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        await _userService.RegisterAsync(userName, passwordHash);

        return token;

    }

    public string CreateToken(string userName)
    {
        try
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, AuthorizeRoles.User)
            };

            var key = new SymmetricSecurityKey(GetBytes(_config.Token));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        catch (Exception ex)
        {
            return null;
        }
    }

    public static  byte[] GetBytes(string secret)
    {

        return System.Text.Encoding.UTF32.GetBytes(secret);
    }


    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(GetBytes(password));
        }
    }



}