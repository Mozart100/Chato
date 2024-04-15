using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class TwoUsersHandShakeScenario : InstructionScenarioBase
{
    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    public TwoUsersHandShakeScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(UserSetups);
        BusinessLogicCallbacks.Add(HandShakeStep);
        SummaryLogicCallback.Add(CleanupStep);
    }


    public override string ScenarioName => "2 people sending Message";

    public override string Description => "Two users sending and listening.";

    private async Task UserSetups()
    {
        await InitializeAsync(Anatoliy_User, Olessya_User);
    }

    private async Task HandShakeStep()
    {
        var message_1 = "Shalom";

        var anatoliySender = InstructionNodeFluentApi.Start(Anatoliy_User).Send(message_1);
        var olessyaReceive = InstructionNodeFluentApi.Start(Olessya_User).Receive(anatoliySender.UserName, message_1);


        var message_2 = "Shalom to you too";

        var olessyaSender = InstructionNodeFluentApi.Start(Olessya_User).Send(message_2);
        var anatoliyReceiver = InstructionNodeFluentApi.Start(Anatoliy_User).Receive(olessyaSender.UserName, message_2);


        anatoliySender.Connect(olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }

    private async Task CleanupStep()
    {
        await UsersCleanup();
    }
}
