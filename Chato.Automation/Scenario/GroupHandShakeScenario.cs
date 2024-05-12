using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Arkovean.Chat.Automation.Scenario;
internal class GroupHandShakeScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";
    private const string Second_Group = "nesher";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";

    public GroupHandShakeScenario(ILogger<GroupHandShakeScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        SetupsLogicCallback.Add(TwoUserSetups);
        BusinessLogicCallbacks.Add(TwoPeopleHandShakeStep);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(_users.Keys.ToArray()));

        BusinessLogicCallbacks.Add(TreeUserSetups);
        BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(_users.Keys.ToArray()));

    }


    public override string ScenarioName => "People sending Message";

    public override string Description => "Users sending and listening.";

    private async Task TwoUserSetups()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User);
    }

    private async Task TwoPeopleHandShakeStep()
    {
        var message_1 = "Hello";
        var groupRoot = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);

        var anatoliySender = groupRoot.SendingBroadcast(Anatoliy_User);
        var olessyaReceive = groupRoot.RecievingFrom(Olessya_User, anatoliySender.UserName);




        var message_2 = "Hello to you too";
        var secondRoot = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_2);

        var olessyaSender = secondRoot.SendingBroadcast(Olessya_User);
        var anatoliyReceiver = secondRoot.RecievingFrom(Anatoliy_User, olessyaSender.UserName);


        anatoliySender.Connect(olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }

    private async Task TreePoepleHandShakeStep()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);
        var secondtGroup = InstructionNodeFluentApi.StartWithGroup(groupName: Second_Group, message_1);



        var anatoliySender = firstGroup.SendingBroadcast(Anatoliy_User);
        var olessyaReceive1 = firstGroup.RecievingFrom(Olessya_User, anatoliySender.UserName);
        var nathanReceive1 = firstGroup.RecievingFrom(Nathan_User, anatoliySender.UserName);
        var nataliRecevier = secondtGroup.Is_Not_Received(Natali_User);


        var message_2 = "Shalom to you too";
        var secondRoot = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_2);


        var olessyaSender = secondRoot.SendingBroadcast(Olessya_User);
        var anatoliyReceiver = secondRoot.RecievingFrom(Anatoliy_User, olessyaSender.UserName);
        var nathanReceiver2 = secondRoot.RecievingFrom(Nathan_User, olessyaSender.UserName);
        var maxReceiver1 = secondRoot.RecievingFrom(Max_User, olessyaSender.UserName);


        anatoliySender.Connect(nataliRecevier, nathanReceive1, olessyaReceive1)
            .Do(maxReceiver1, async user =>
            {
                await RegisterUsers(Max_User);
                await AssignUserToGroupAsync(First_Group, Max_User);
            })
            .Verificationn(Max_User, anatoliySender)
            .Connect(olessyaSender).Connect(anatoliyReceiver, nathanReceiver2, maxReceiver1);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }

   

    private async Task TreeUserSetups()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Natali_User);

        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
        await AssignUserToGroupAsync(Second_Group, Natali_User);
    }
}
