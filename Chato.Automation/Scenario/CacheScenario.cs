using Chato.Automation.Infrastructure.Instruction;
using Chatto.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class CacheScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";


    public CacheScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {
        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(SendingInsideTheRoom);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));

    }

    public override string ScenarioName => "Only Persistent cache remains.";
    public override string Description => "All cache except persistent removed.";

    private async Task SendingInsideTheRoom()
    {
        var max = 20;
        for (int i = 0; i < max; i++)
        {
            await Task.Delay(1000);
            Logger.LogInformation($"Delayed {i + 1}/{max} second.");
        }

        var token = Users[Anatoliy_User].RegisterResponse.Token;
        var response = await Get<ResponseWrapper<GetAllRoomResponse>>(GetAllRoomsUrl, token);
        response.Body.Rooms.Should().BeNull();


        //var message_1 = "Shalom";
        //var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        //var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
        //var olessyaSender = firstGroup.ReceivingFrom(Olessya_User, anatoliySender.UserName);
        //var nathanReceiver = firstGroup.ReceivingFrom(Nathan_User, anatoliySender.UserName);

        //var maxReceiver = firstGroup.Is_Not_Received(Max_User);


        //anatoliySender.Connect(olessyaSender, nathanReceiver, maxReceiver);

        //var graph = new InstructionGraph(anatoliySender);
        //await InstructionExecuter(graph);
    }




    private async Task Setup_SendingInsideTheRoom_Step()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }


}
