using Chato.Automation.Infrastructure.Instruction;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class BasicScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";

    public BasicScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        BusinessLogicCallbacks.Add(SetupGroup);
        BusinessLogicCallbacks.Add(SendingToSpecificPerson);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


        BusinessLogicCallbacks.Add(SetupGroup);
        BusinessLogicCallbacks.Add(VerificationStep);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


        BusinessLogicCallbacks.Add(Setup_SendingOnlyToRoomStep_Step);
        BusinessLogicCallbacks.Add(SendingOnlyToRoomStep);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));



        //SummaryLogicCallback.Add(CheckAllCleaned);

    }

    private async Task CheckAllCleaned()
    {
        var roomInfo = await Get<GetAllRoomResponse>(RoomsControllerUrl);
        roomInfo.Rooms.Should().HaveCount(0);
    }

    public override string ScenarioName => "All sort of mini scenarios";

    public override string Description => "To run mini scenarios.";

    private async Task SendingToSpecificPerson()
    {
        var message_1 = "Shalom";
        //var users = InstructionNodeFluentApi.StartWithLobi(Anatoliy_User,Olessya_User,Nathan_User);
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);

        //var sending = users[Anatoliy_User].SendingTo(Anatoliy_User, Nathan_User);

        var anatoliySender = firstGroup.SendingTo(Anatoliy_User, Nathan_User);
        var nathanReceive1 = firstGroup.ReceivingFrom(Nathan_User, anatoliySender.UserName);
        var olessyaReceive1 = firstGroup.Is_Not_Received(Olessya_User);


        anatoliySender.Connect(nathanReceive1, olessyaReceive1);//.GetGroupInfo(firstGroup.GroupName);
        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);

    }

    private async Task VerificationStep()
    {

        //var roomInfo = await Get<GetAllRoomResponse>(RoomsControllerUrl);
        //roomInfo.Rooms.Should().HaveCount(0);


        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
        var olessyaSender = firstGroup.SendingToRestRoom(Olessya_User);
        var maxReceiver1 = firstGroup.ReceivingFrom(Max_User, olessyaSender.UserName);

        anatoliySender.Connect(olessyaSender)
            .Do(maxReceiver1, async user =>
            {
                await RegisterUsers(Max_User);
                await AssignUserToGroupAsync(First_Group, Max_User);
            })
            .ReceivedVerification(Max_User, anatoliySender, olessyaSender);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }


    private async Task SendingOnlyToRoomStep()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
        var olessyaSender = firstGroup.ReceivingFrom(Olessya_User, anatoliySender.UserName);
        var nathanReceiver = firstGroup.ReceivingFrom(Nathan_User, anatoliySender.UserName);

        var maxReceiver = firstGroup.Is_Not_Received(Max_User);



        anatoliySender.Connect(olessyaSender, nathanReceiver, maxReceiver);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }


    private async Task SetupGroup()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }

    private async Task Setup_SendingOnlyToRoomStep_Step()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }

}
