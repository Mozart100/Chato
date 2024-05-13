using Chato.Server.Infrastracture;
using Chato.Server.Models.Dtos;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Chato.Server.Services;

public class AuthenticationService
{
    private readonly AppSettingConfig _config;

    public AuthenticationService(IOptions<AppSettingConfig> config)
    {
        _config = config.Value;
    }


    public RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }


    public void SetRefreshToken(RefreshToken newRefreshToken, HttpResponse response)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };

        response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

        //user.RefreshToken = newRefreshToken.Token;
        //user.TokenCreated = newRefreshToken.Created;
        //user.TokenExpires = newRefreshToken.Expires;
    }


}
