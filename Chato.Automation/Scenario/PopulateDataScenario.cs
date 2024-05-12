using Chato.Automation.Responses;
using Chato.Automation.Scenario;
using Chato.Server.Models.Dtos;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Arkovean.Chat.Automation.Scenario;

internal class PopulateDataScenario : InstructionScenarioBase
{
    private const string Haifa_Room = "haifa";

    private const string haifa_user1 = $"{Haifa_Room}_user1";
    private const string haifa_user2 = $"{Haifa_Room}_user2";
    private const string haifa_user3 = $"{Haifa_Room}_user3";

    private readonly List<string> _localUsers;

    public PopulateDataScenario(ILogger<PopulateDataScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        //SetupsLogicCallback.Add(TwoUserSetups);
        //BusinessLogicCallbacks.Add(TwoPeopleHandShakeStep);

        //BusinessLogicCallbacks.Add(() => GroupUsersCleanup(First_Group));
        BusinessLogicCallbacks.Add(PopulateUsers);
        //BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);
        //BusinessLogicCallbacks.Add(() => GroupUsersCleanup(First_Group, Second_Group));


        _localUsers = new List<string> { haifa_user1, haifa_user2, haifa_user3 };

    }


    public override string ScenarioName => "Populatin real data";

    public override string Description => "Populating real data so client can use.";


    public async Task PopulateUsers()
    {

        var roomInfo = await Get<GetAllRoomResponse>(RoomsControllerUrl);
        roomInfo.Rooms.Should().HaveCount(0);

        foreach (var user in _localUsers)
        {
            var registrationRequest = new RegisterAndLoginRequest { Password = "string", Username = user };
            var registrationInfo = await RunPostCommand<RegisterAndLoginRequest, RegisterResponse>(RegisterAuthControllerUrl, registrationRequest);
            var tokenResponse = await RunPostCommand<RegisterAndLoginRequest, LoginResponse>(LoginAuthControllerUrl, registrationRequest);
        }



        roomInfo = await Get<GetAllRoomResponse>(RoomsControllerUrl);
        roomInfo.Rooms.Should().HaveCount(0);
    }


}
