using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class FullHandShakeScenario : InstructionScenarioBase
{
    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";

    public FullHandShakeScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(TwoUserSetups);
        BusinessLogicCallbacks.Add(TwoPeopleHandShakeStep);
        BusinessLogicCallbacks.Add(UsersCleanup);
        BusinessLogicCallbacks.Add(TreeUserSetups);
        BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);
    }


    public override string ScenarioName => "People sending Message";

    public override string Description => "Users sending and listening.";

    private async Task TwoUserSetups()
    {
        await InitializeAsync(Anatoliy_User, Olessya_User);
    }

    private async Task TwoPeopleHandShakeStep()
    {
        var message_1 = "Hello";

        var anatoliySender = InstructionNodeFluentApi.Start(Anatoliy_User).Send(message_1);
        var olessyaReceive = InstructionNodeFluentApi.Start(Olessya_User).Receive(anatoliySender.UserName, message_1);


        var message_2 = "Hello to you too";

        var olessyaSender = InstructionNodeFluentApi.Start(Olessya_User).Send(message_2);
        var anatoliyReceiver = InstructionNodeFluentApi.Start(Anatoliy_User).Receive(olessyaSender.UserName, message_2);


        anatoliySender.Connect(olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }

    private async Task TreeUserSetups()
    {
        await InitializeAsync(Anatoliy_User, Olessya_User, Nathan_User);
    }

    private async Task TreePoepleHandShakeStep()
    {
        var message_1 = "Shalom";

        var anatoliySender = InstructionNodeFluentApi.Start(Anatoliy_User).Send(message_1);
        var olessyaReceive = InstructionNodeFluentApi.Start(Olessya_User).Receive(anatoliySender.UserName, message_1);
        var nathanReceiver1 = InstructionNodeFluentApi.Start(Nathan_User).Receive(anatoliySender.UserName, message_1);

        anatoliySender.Connect(nathanReceiver1, olessyaReceive);


        var message_2 = "Shalom to you too";

        var olessyaSender = InstructionNodeFluentApi.Start("olessya").Send(message_2);
        var anatoliyReceiver = InstructionNodeFluentApi.Start("anatoliy").Receive(olessyaSender.UserName, message_2);
        var nathanReceiver2 = InstructionNodeFluentApi.Start("nathan").Receive(olessyaSender.UserName, message_2);


        anatoliySender.Connect(nathanReceiver1, olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver, nathanReceiver2);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }
}
