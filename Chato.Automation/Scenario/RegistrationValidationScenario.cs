using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Services;
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

        //BusinessLogicCallbacks.Add(InvaliRegistrationValidationStep);
        BusinessLogicCallbacks.Add(UploadingFiles);

    }


    public override string ScenarioName => "Registration Validation";

    public override string Description => "Ensuring mandatory fields are inserted during registration.";


    private async Task InvaliRegistrationValidationStep()
    {
        var request = new RegistrationRequest { UserName = Anatoliy_User, Age = 20, Description = $"Description_{Anatoliy_User}" };
        await RegisterUser(request);


        request = new RegistrationRequest { UserName = Anatoliy_User, Age = 10, Description = $"Description_{Anatoliy_User}", Gender = "male" };
        await RegisterUser(request);

    }


    private async Task UploadingFiles()
    {
        var message_1 = "Shalom";
        var supervisor = $"User_{nameof(UploadingFiles)}";

        await RegisterUsers(Anatoliy_User);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User);

        users[Anatoliy_User].Step(users[Anatoliy_User].Do(async user =>
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "test.jpeg");
            var files = new[] { path, path };
            var token = Users[Anatoliy_User].RegisterResponse.Token;



            var response = await UploadFiles<ResponseWrapper<UploadDocumentsResponse>>(UserUploadFilesUrl, token, files);
            //response.Body.Document1.Should().BeTrue();
            //response.Body.Document2.Should().BeTrue();
            //response.Body.Document3.Should().BeFalse();
            //response.Body.Document4.Should().BeFalse();
            //response.Body.Document5.Should().BeFalse();

            response.Body.Files.Count.Should().Be(2);

            var allImages = await Get<ResponseWrapper<GetAllUserImagesResponse>>(GetAllUserimagesUrl, token);

            foreach (var file in allImages.Body.Files)
            {
                file.Should().NotContain("C:");
                file.Should().Contain(IUserService.UserChatImage);
                var ptr = await GetImage(ImagePathCombineWithWwwroot(file));
                ptr.Should().NotBeNull();
            }

            //var parameters = new Dictionary<string, string> { { "num", "1" } };
            //var fileContent = await Get<byte[]>(DownloadFileUrl, token, parameters);
            //fileContent.Should().NotBeNull();



        }))
            .Step(users[Anatoliy_User].Logout())
            ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

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
