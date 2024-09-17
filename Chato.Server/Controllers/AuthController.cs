using Chato.Server.FeatureFlags;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System.IO;

namespace Chato.Server.Controllers;


public class AuthorizeRoleConsts
{
    public const string User = "User";
}

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public const string RegistrationUrl = "register";
    public const string UploadUrl = "upload";
    public const string DownloadUrl = "download";

    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IFeatureManager _featureManager;

    public AuthController(IUserService userService, IAuthenticationService authenticationService, 
        IFeatureManager  featureManager)
    {
        _userService = userService;
        this._authenticationService = authenticationService;
        this._featureManager = featureManager;
    }

    [Route("status")]
    [HttpGet]
    public IActionResult Status()
    {
        return Ok();
    }

    [Route(RegistrationUrl)]
    [HttpPost]
    public async Task<RegistrationResponse> Register([FromBody] RegistrationRequest request)
    {
        var flag = await _featureManager.IsEnabledAsync(FeatureFlagConsts.ClipArticale);

        var token = await _authenticationService.RegisterAsync(request);
        return new RegistrationResponse { Token = token, UserName = request.UserName , Description = request.Description , Age = request.Age, Gender  = request.Gender };
    }



    [Route(UploadUrl)]
    [HttpPost, Authorize]
    public async Task<ActionResult<UploadDocumentsResponse>> Upload(IEnumerable<IFormFile> documents)
    {
        var userName = User.Identity.Name;
        var response = await _authenticationService.UploadFilesAsync(userName, documents);

        return Ok(response);
    }

    [Route(DownloadUrl)]
    [HttpGet, Authorize]
    public async Task<IActionResult> Download([FromQuery] int num)
    {
        var userName = User.Identity.Name;
        var response = await _authenticationService.DownloadFilesAsync(userName);
        var firstFile = response.ElementAt(num);

        return File(firstFile.Content, "application/octet-stream", firstFile.FileName);
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
