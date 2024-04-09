using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class TwoUsersListenChatScenario : HubScenarioBase
{
    private UserHubChat _user1;
    private UserHubChat _user2;

    public TwoUsersListenChatScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(UserSetups);
        BusinessLogicCallbacks.Add(ListenStep);
        SummaryLogicCallback.Add(Cleannup);
    }


    public override string ScenarioName => "ListenScenario";
    public override string Description => "Testing connectivity of the hub";

    private async Task UserSetups()
    {
        _user1 = new UserHubChat(this, "anatoliy");
        _user2 = new UserHubChat(this, "nathan");

        AddUsers( _user1 , _user2);

        for (int i = 0; i < 5; i++)
        {
            _user1.AddRecieveInstruction();
            _user2.AddRecieveInstruction();
        }

        //await Task.Delay(5 * 1000);
        await Connection.StartAsync();
    }


    private async Task ListenStep()
    {
        StartSignal.Release();

        await Listen();

        await FinishedSignal.WaitAsync();
    }

    private async Task Cleannup()
    {
        if (Connection.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Disconnected)
        {
            await Connection.StopAsync();
        }
    }
}
