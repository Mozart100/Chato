using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Services;
using Chatto.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class CacheScenario : InstructionScenarioBase
{
    private const string First_Group = nameof(CacheScenario);

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";


    public CacheScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {
        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(UnusedCache_Preloning);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));



        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(UnusedCacheEvicted);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));



        //BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        //BusinessLogicCallbacks.Add(AbsoluteEviction);
        //BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));

    }

    public override string ScenarioName => "Only Persistent cache remains.";
    public override string Description => "All cache except persistent removed.";


    private async Task UnusedCache_Preloning()
    {
        var token = Users[Anatoliy_User].RegisterResponse.Token;
        //var max = 8;
        //for (int i = 0; i < max; i++)
        //{
        //    var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);

        //    var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.RoomName == nameof(CacheScenario));

        //    cacheScenarioRoom.RoomName.Should().NotBeNull();

        //    await Task.Delay(1000);
        //    Logger.LogInformation($"Delayed {i + 1}/{max} second.");
        //}

        await CountDown(async () =>
        {
            var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);

            var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.RoomName == nameof(CacheScenario));

            cacheScenarioRoom.RoomName.Should().NotBeNull();
        }, 8);


    }

    private async Task UnusedCacheEvicted()
    {
        var token = Users[Anatoliy_User].RegisterResponse.Token;
        var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.RoomName == nameof(CacheScenario));

        await CountDown();

        response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        response.Body.Rooms.Count().Should().Be(1);
        response.Body.Rooms.First().RoomName.Should().Be(IPersistentUsers.DefaultRoom);


    }

    private async Task AbsoluteEviction()
    {
        var token = Users[Anatoliy_User].RegisterResponse.Token;
        var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        var cacheScenarioRoom = response.Body.Rooms.FirstOrDefault(x => x.RoomName == nameof(CacheScenario));

        await CountDown(60);

        response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        response.Body.Rooms.Count().Should().Be(1);
        response.Body.Rooms.First().RoomName.Should().Be(IPersistentUsers.DefaultRoom);


    }


    private async Task Setup_SendingInsideTheRoom_Step()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }


}
