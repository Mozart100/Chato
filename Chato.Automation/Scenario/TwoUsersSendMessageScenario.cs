using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;
using FluentAssertions.Equivalency;

namespace Arkovean.Chat.Automation.Scenario;

internal class TwoUsersSendMessageScenario : HubScenarioBase
{
    private UserHubChat _anatoliy;
    private UserHubChat _olessya;

    public TwoUsersSendMessageScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(UserSetups);
        BusinessLogicCallbacks.Add(SendingStep);
        //SummaryLogicCallback.Add(Cleannup);
    }


    public override string ScenarioName => "Sending Message";

    public override string Description => "Two users sending messages";

    private async Task UserSetups()
    {
        var message_1 = "Shalom";

        var anatoliySender = InstructionNodeFluentApi.Start("anatoiiy").Send(message_1);
        var olessyaReceive = InstructionNodeFluentApi.Start("olessya").Receive(anatoliySender.UserName, message_1);


        var message_2 = "Shalom to you too";

        var olessyaSender = InstructionNodeFluentApi.Start("olessya").Send(message_2);
        var anatoliyReceiver = InstructionNodeFluentApi.Start("anatoliy").Receive( olessyaSender.UserName, message_2);


        anatoliySender.Connect(olessyaReceive).Connect(olessyaSender).Connect(anatoliyReceiver);

        await Connection.StartAsync();
    }

    private async Task SendingStep()
    {
        StartListeningSignal.Release();

        await Listen();

        await FinishedSignal.WaitAsync();
    }
}
