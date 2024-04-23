using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class GroupHandShakeScenario : InstructionScenarioBase
{
    private const string Group_Name = "arkove";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";

    public GroupHandShakeScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(TwoUserSetups);
        BusinessLogicCallbacks.Add(TwoPeopleHandShakeStep);

        BusinessLogicCallbacks.Add(GroupUsersCleanup);
        BusinessLogicCallbacks.Add(TreeUserSetups);
        BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);

    }


    public override string ScenarioName => "People sending Message";

    public override string Description => "Users sending and listening.";

    private async Task TwoUserSetups()
    {
        await InitializeWithGroupAsync(Group_Name, Anatoliy_User, Olessya_User);
    }

    private async Task TwoPeopleHandShakeStep()
    {
        var message_1 = "Hello";

        var anatoliySender = InstructionNodeFluentApi.Start(Anatoliy_User,Group_Name).Send(message_1);
        var olessyaReceive = InstructionNodeFluentApi.Start(Olessya_User, Group_Name).Receive(anatoliySender.UserName, message_1);


        var message_2 = "Hello to you too";

        var olessyaSender = olessyaReceive.ReplicateNameAndGroup().Send(message_2);
        var anatoliyReceiver = anatoliySender.ReplicateNameAndGroup().Receive(olessyaSender.UserName, message_2);


        anatoliySender.Connect(olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);

    }

    private async Task TreePoepleHandShakeStep()
    {
        var message_1 = "Shalom";

        var anatoliySender = InstructionNodeFluentApi.Start(Anatoliy_User, Group_Name).Send(message_1);
        var olessyaReceive1 = InstructionNodeFluentApi.Start(Olessya_User, Group_Name).Receive(anatoliySender.UserName, message_1);
        var nathanReceive1 = InstructionNodeFluentApi.Start(Nathan_User, Group_Name).Receive(anatoliySender.UserName, message_1);
        var nataliReceive = InstructionNodeFluentApi.Start(Natali_User, Group_Name).Not_Receive();



        anatoliySender.Connect(nathanReceive1, olessyaReceive1);


        var message_2 = "Shalom to you too";

        var olessyaSender = olessyaReceive1.ReplicateNameAndGroup().Send(message_2);
        var anatoliyReceiver = anatoliySender.ReplicateNameAndGroup().Receive(olessyaSender.UserName, message_2);
        var nathanReceiver2 = nathanReceive1.ReplicateNameAndGroup().Receive(olessyaSender.UserName, message_2);


        anatoliySender.Connect(nathanReceive1, olessyaReceive1).Connect(olessyaSender).Connect(anatoliyReceiver, nathanReceiver2);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }

    public async Task GroupUsersCleanup()
    {
        await GroupUsersCleanup(Group_Name);
    }

    private async Task TreeUserSetups()
    {
        await InitializeWithGroupAsync(Group_Name, Anatoliy_User, Olessya_User, Nathan_User);
    }

}
