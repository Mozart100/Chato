using Chato.Server.Models.Dtos;
using Chato.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chato.Server.Controllers;

public class AuthorizeRoles
{
    public const string User = "User";
}


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IUserService userService, IAuthenticationService authenticationService )
    {
        _userService = userService;
        this._authenticationService = authenticationService;
    }

    [HttpGet, Authorize]
    public ActionResult<string> GetMe()
    {
        var userName = _userService.GetMyName();
        return Ok(userName);
    }

    [Route("register")]
    [HttpPost]
    public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
    {
        var token = await _authenticationService.RegisterAsync(request.UserName,request.Password);
        return Ok(new RegistrationResponse { Token = token, UserName = request.UserName });
    }

    #region Feature  development
    //[Route("register")]
    //[HttpPost]
    //public async Task<ActionResult<UserResponse>> Register(UserRequest request)
    //{
    //    CreatePasswordHash(request.PasswordHash, out byte[] passwordHash, out byte[] passwordSalt);

    //    LastUser.Username = request.Username;
    //    LastUser.PasswordHash = passwordHash;
    //    LastUser.PasswordSalt = passwordSalt;

    //    await _userService.RegisterAsync(request.Username, passwordHash, passwordSalt);


    //    return Ok(LastUser);
    //}

    //[HttpPost]
    //[Route("login")]

    //public async Task<ActionResult<LoginResponse>> Login(UserRequest request)
    //{
    //    if (LastUser.Username != request.Username)
    //    {
    //        return BadRequest("User not found.");
    //    }

    //    if (!VerifyPasswordHash(request.PasswordHash, LastUser.PasswordHash, LastUser.PasswordSalt))
    //    {
    //        return BadRequest("Wrong password.");
    //    }

    //    string token = CreateToken(LastUser);

    //    var refreshToken = GenerateRefreshToken();
    //    SetRefreshToken(refreshToken);

    //    await _userService.LoginAsync(request.Username, request.PasswordHash);

    //    return Ok(new LoginResponse { Token = token});
    //}

    //[HttpPost("refresh-token")]
    //public async Task<ActionResult<string>> RefreshToken()
    //{
    //    var refreshToken = Request.Cookies["refreshToken"];

    //    if (!LastUser.RefreshToken.Equals(refreshToken))
    //    {
    //        return Unauthorized("Invalid Refresh Token.");
    //    }
    //    else if (LastUser.TokenExpires < DateTime.Now)
    //    {
    //        return Unauthorized("Token expired.");
    //    }

    //    string token = CreateToken(LastUser);
    //    var newRefreshToken = GenerateRefreshToken();
    //    SetRefreshToken(newRefreshToken);

    //    return Ok(token);
    //}

    //private RefreshToken GenerateRefreshToken()
    //{
    //    var refreshToken = new RefreshToken
    //    {
    //        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
    //        Expires = DateTime.Now.AddDays(7),
    //        Created = DateTime.Now
    //    };

    //    return refreshToken;
    //}

    //private void SetRefreshToken(RefreshToken newRefreshToken)
    //{
    //    var cookieOptions = new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Expires = newRefreshToken.Expires
    //    };
    //    Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

    //    LastUser.RefreshToken = newRefreshToken.Token;
    //    LastUser.TokenCreated = newRefreshToken.Created;
    //    LastUser.TokenExpires = newRefreshToken.Expires;
    //}

    //private string CreateToken(UserResponse user)
    //{
    //    try
    //    {

    //    var dbValue = _configuration.GetSection("AppSettings:Token").Value;

    //    List <Claim> claims = new List<Claim>
    //    {
    //        new Claim(ClaimTypes.Name, user.Username),
    //        new Claim(ClaimTypes.Role, AuthorizeRoles.User)
    //    };

    //        //var key = new SymmetricSecurityKey(GenerateHmacSha512Key());

    //        var key = new SymmetricSecurityKey(GetBytes(_configuration.GetSection("AppSettings:Token").Value));

    //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    //    var token = new JwtSecurityToken(
    //        claims: claims,
    //        expires: DateTime.Now.AddDays(1),
    //        signingCredentials: creds);

    //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

    //    return jwt;
    //    }

    //    catch (Exception ex)
    //    {
    //        return null;
    //    }
    //}

    //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    //{
    //    using (var hmac = new HMACSHA512())
    //    {
    //        passwordSalt = hmac.Key;
    //        passwordHash = hmac.ComputeHash(GetBytes(password));
    //    }
    //}

    //private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    //{
    //    using (var hmac = new HMACSHA512(passwordSalt))
    //    {
    //        var computedHash = hmac.ComputeHash(GetBytes(password));
    //        return computedHash.SequenceEqual(passwordHash);
    //    }
    //}


    //private byte[] GetBytes(string secret)
    //{

    //    return System.Text.Encoding.UTF32.GetBytes(secret);
    //} 
    #endregion
}
