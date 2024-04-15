using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class ThreeUsersHandShakeScenario : InstructionScenarioBase
{
    private const string Anatoliy_User = "anatoiiy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";

    public ThreeUsersHandShakeScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(UserSetups);
        BusinessLogicCallbacks.Add(HandShakeStep);
    }

    public override string ScenarioName => "3 People speaking";
    public override string Description => "3 People speaking and listening.";

    private async Task UserSetups()
    {
        await InitializeAsync(Anatoliy_User,Olessya_User,Nathan_User);
    }

    

    private async Task HandShakeStep()
    {
        var message_1 = "Shalom";

        var anatoliySender = InstructionNodeFluentApi.Start(Anatoliy_User).Send(message_1);
        var olessyaReceive = InstructionNodeFluentApi.Start(Olessya_User).Receive(anatoliySender.UserName, message_1);
        var nathanReceiver1 = InstructionNodeFluentApi.Start(Nathan_User).Receive(anatoliySender.UserName, message_1);

        anatoliySender.Connect(nathanReceiver1, olessyaReceive);


        //var message_2 = "Shalom to you too";

        //var olessyaSender = InstructionNodeFluentApi.Start("olessya").Send(message_2);
        //var anatoliyReceiver = InstructionNodeFluentApi.Start("anatoliy").Receive(olessyaSender.UserName, message_2);
        //var nathanReceiver2 = InstructionNodeFluentApi.Start("nathan").Receive(olessyaSender.UserName, message_2);


        //anatoliySender.Connect(nathanReceiver1, olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver, nathanReceiver2);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }

}
