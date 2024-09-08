using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Configuration;
using Chato.Server.Services;
using Chato.Server.Utilities;
using Chatto.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class CacheScenario : InstructionScenarioBase
{
    private const int Forgiveness = 4;

    private const string First_Group = nameof(CacheScenario);

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";


    private CacheEvictionRoomConfigDto _evictionConfig;

    public CacheScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        SetupsLogicCallback.Add(GetConfigurations_Step);

        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(UnusedCache_Preloning);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));



        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(UnusedCacheEvicted);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));



        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(AbsoluteEviction);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


    }

    private async Task GetConfigurations_Step()
    {
        _evictionConfig = await GetEvictionConfigurationAsync();
    }

    public override string ScenarioName => "Only Persistent cache remains.";
    public override string Description => "All cache except persistent removed.";


    private async Task UnusedCache_Preloning()
    {
        var token = Users[Anatoliy_User].RegisterResponse.Token;

        var tolerence = CacheEvictionUtility.ConvertToTimeSpan(_evictionConfig.TimeMeasurement, _evictionConfig.UnusedTimeout + Forgiveness);
        var amountTicks = (int)tolerence.TotalSeconds;

        await CountDown(async (int tick) =>
        {
            var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
            var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.ChatName == nameof(CacheScenario));
            cacheScenarioRoom.ChatName.Should().NotBeNull();
        }, amountTicks);


    }

    private async Task UnusedCacheEvicted()
    {
        var token = Users[Anatoliy_User].RegisterResponse.Token;
        var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.ChatName == nameof(CacheScenario));


        var tolerence = CacheEvictionUtility.ConvertToTimeSpan(_evictionConfig.TimeMeasurement, _evictionConfig.UnusedTimeout + Forgiveness);
        var amountTicks = (int)tolerence.TotalSeconds;

        await CountDown(amountTicks);

        response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        response.Body.Rooms.Count().Should().Be(IPersistentUsers.PersistentUsers.Count());
        response.Body.Rooms.First().ChatName.Should().Be(IPersistentUsers.AdultRoom);


    }

    private async Task AbsoluteEviction()
    {
        var token = Users[Anatoliy_User].RegisterResponse.Token;

        await CountDown(async (int tick) =>
        {
            var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);

            //var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.RoomName == nameof(CacheScenario));
            //cacheScenarioRoom.RoomName.Should().NotBeNull();
        }, 50);

    }


    private async Task Setup_SendingInsideTheRoom_Step()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }


}
