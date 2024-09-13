using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using Chato.Server.Services;
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

        //BusinessLogicCallbacks.Add(SetupGroup);
        BusinessLogicCallbacks.Add(SendingWithinLobi);
        BusinessLogicCallbacks.Add(SendingWithinLobi_UserMovedChat);
        //BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));

        


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

    private async Task SendingWithinLobi()
    {
        await RegisterUsers2222(Anatoliy_User, Olessya_User, Nathan_User);

        var message_1 = "Shalom";
        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);

        users[Anatoliy_User].Connect(users[Nathan_User]).Connect(users[Olessya_User])
            .Connect(users[Anatoliy_User].SendingToRestRoom222(message_1, IChatService.Lobi))
            .Connect(users[Nathan_User].ReceivingFrom2222(Anatoliy_User, message_1))
            .Connect(users[Olessya_User].ReceivingFrom2222(Anatoliy_User, message_1))
            .Connect(users[Anatoliy_User].Logout())
            .Connect(users[Olessya_User].Logout())
            .Connect(users[Nathan_User].Logout());


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

    }

    private async Task SendingWithinLobi_UserMovedChat()
    {
        await RegisterUsers2222(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "university_chat";

        var message_1 = "Shalom";
        var message_2 = "hello";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Connect(users[Nathan_User]).Connect(users[Olessya_User])
            .Connect(users[Anatoliy_User].SendingToRestRoom222(Anatoliy_User, message_1, IChatService.Lobi))
            .Connect(users[Nathan_User].ReceivingFrom2222(Anatoliy_User, message_1))
            .Connect(users[Olessya_User].ReceivingFrom2222(Anatoliy_User, message_1))
            .Connect(users[Olessya_User].JoinOrCreateChat(chat2))
            .Connect(users[Nathan_User].JoinOrCreateChat(chat2))

            .Connect(users[Anatoliy_User].SendingToRestRoom222(Anatoliy_User, message_1, IChatService.Lobi))
            .Connect(users[Olessya_User].ReceivingFrom2222(Anatoliy_User, message_1))

            .Connect(users[Nathan_User].SendingToRestRoom222(message_2, chat2))
            .Connect(users[Olessya_User].ReceivingFrom2222(Nathan_User, message_1))


            ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

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
