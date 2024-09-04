using Chatto.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class RegistrationValidationScenario : InstructionScenarioBase
{
    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";

    public RegistrationValidationScenario(ILogger<RegistrationValidationScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        BusinessLogicCallbacks.Add(SetupGroupStep);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Anatoliy_User));
    }


    public override string ScenarioName => "Registration Validation";

    public override string Description => "Ensuring mandatory fields are inserted during registration.";


    private async Task SetupGroupStep()
    {
        var request = new RegistrationRequest { UserName = Anatoliy_User, Age = 20, Description = $"Description_{Anatoliy_User}" };
        await RegisterUser(request);


        request = new RegistrationRequest { UserName = Anatoliy_User, Age = 10, Description = $"Description_{Anatoliy_User}", Gender = "male" };
        await RegisterUser(request);


        await RegisterUsers(Anatoliy_User);


        request = new RegistrationRequest { UserName = Anatoliy_User, Age = 20, Description = $"Description_{Anatoliy_User}", Gender = "male" };
        await RegisterUser(request);


        var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "test.jpeg");
        var files = new[] { path, path };
        var token = Users[Anatoliy_User].RegisterResponse.Token;


        var response = await UploadFiles<ResponseWrapper<UploadDocumentsResponse>>(UploadFilesUrl, token, files);

        response.Body.Document1.Should().BeTrue();
        response.Body.Document2.Should().BeTrue();
        response.Body.Document3.Should().BeFalse();
        response.Body.Document4.Should().BeFalse();
        response.Body.Document5.Should().BeFalse();


        var parameters = new Dictionary<string, string> { { "num", "1" } };
        var fileContent = await Get<byte[]>(DownloadFileUrl, token,parameters);
        fileContent.Should().NotBeNull();

    }

    private async Task RegisterUser(RegistrationRequest request)
    {
        var isExceptionOccurred = false;
        try
        {
            var registrationInfo = await RunPostCommand<RegistrationRequest, ResponseWrapper<RegistrationResponse>>(RegisterAuthControllerUrl, request);
        }
        catch (Exception ex)
        {
            isExceptionOccurred = true;
        }

        isExceptionOccurred.Should().BeTrue();
    }

}
