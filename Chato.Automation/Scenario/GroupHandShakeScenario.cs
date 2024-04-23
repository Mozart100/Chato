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
        var groupRoot = InstructionNodeFluentApi.StartWithGroup(groupName: Group_Name, message_1);

        var anatoliySender = groupRoot.IsSender(Anatoliy_User);
        var olessyaReceive = groupRoot.IsReciever(Olessya_User, anatoliySender.UserName);




        var message_2 = "Hello to you too";
        var secondRoot = InstructionNodeFluentApi.StartWithGroup(groupName: Group_Name, message_2);

        var olessyaSender = secondRoot.IsSender(Olessya_User);
        var anatoliyReceiver = secondRoot.IsReciever(Anatoliy_User, olessyaSender.UserName);


        anatoliySender.Connect(olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }

    private async Task TreePoepleHandShakeStep()
    {
        var message_1 = "Shalom";
        var groupRoot = InstructionNodeFluentApi.StartWithGroup(groupName: Group_Name, message_1);


        var anatoliySender = groupRoot.IsSender(Anatoliy_User);
        var olessyaReceive1 = groupRoot.IsReciever(Olessya_User, anatoliySender.UserName);
        var nathanReceive1 = groupRoot.IsReciever(Nathan_User, anatoliySender.UserName);


        var message_2 = "Shalom to you too";
        var secondRoot = InstructionNodeFluentApi.StartWithGroup(groupName: Group_Name, message_2);


        var olessyaSender = secondRoot.IsSender(Olessya_User);
        var anatoliyReceiver = secondRoot.IsReciever(Anatoliy_User, olessyaSender.UserName);
        var nathanReceiver2 = secondRoot.IsReciever(Nathan_User, olessyaSender.UserName);


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
