using Chato.Server.Controllers;
using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture;
using Chato.Server.Services.Validations;
using Chatto.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Chato.Server.Services;

public interface IAuthenticationService
{
    //void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    string CreateToken(string user);
    //Task<string> RegisterAsync(string userName, string password, string description, string gender, int age);
    Task<string> RegisterAsync(RegistrationRequest request);
    Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<IFormFile> documents);
    Task<IEnumerable<UserFileInfo>> DownloadFilesAsync(string userName);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfig _config;
    private readonly IUserService _userService;
    private readonly IRegistrationValidationService _registrationValidationService;

    public AuthenticationService(IOptions<AuthenticationConfig> config,
        IUserService userService,
        IRegistrationValidationService registrationValidationService)
    {
        _config = config.Value;

        this._userService = userService;
        this._registrationValidationService = registrationValidationService;
    }

    public async Task<string> RegisterAsync(RegistrationRequest request)
    {
        await _registrationValidationService.RegistrationRequestValidateAsync(request);
        return await RegisterAsync(request.UserName,  request.Description, request.Gender, request.Age);
    }
    public async Task<IEnumerable<UserFileInfo>> DownloadFilesAsync(string userName)
    {
        var files = await _userService.DownloadFilesAsync(userName);

        return files;
    }
    public async Task<UploadDocumentsResponse> UploadFilesAsync(string userName, IEnumerable<IFormFile> documents)
    {
        var data = new List<UserFileInfo>(); 
        
        foreach (var document in documents)
        {
            using (var memoryStream = new MemoryStream())
            {
                await document.CopyToAsync(memoryStream);
                var documentBytes = memoryStream.ToArray();
                data.Add(new UserFileInfo(document.FileName, documentBytes));
            }
        }

        return await _userService.UploadFilesAsync(userName, data);
    }


    public string CreateToken(string userName)
    {
        try
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, AuthorizeRoleConsts.User)
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

    public static byte[] GetBytes(string secret)
    {
        return System.Text.Encoding.UTF32.GetBytes(secret);
    }

    public static string GetMessage(byte[] ptr)
    {
        return System.Text.Encoding.UTF32.GetString(ptr);
    }


    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(GetBytes(password));
        }
    }

    private async Task<string> RegisterAsync(string userName, string description, string gender, int age)
    {
        var token = CreateToken(userName);

        //CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        await _userService.RegisterAsync(userName, description, gender, age);
        //await _userService.RegisterAsync(userName, passwordHash, descrkccption, gender, age);

        return token;

    }
}
