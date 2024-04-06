using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class PingChatScenario : HubScenarioBase
{
    private UserHubChat _user1;
    private UserHubChat _user2;

    private SemaphoreSlim _signal;

    public PingChatScenario(string baseUrl) : base(baseUrl)
    {
        _signal = new SemaphoreSlim(1);

        SetupsLogicCallback.Add(UserSetups);
        BusinessLogicCallbacks.Add(PingHub);
    }

    private async Task UserSetups()
    {
        _user1 = new UserHubChat(this, "anatoliy");
        _user2 = new UserHubChat(this, "nathan");


        _user1.Finished += _user1_Finished;

        Users.Add(_user1.Name, _user1);
        Users.Add(_user2.Name, _user2);

        for (int i = 0; i < 5; i++)
        {
            _user1.AddRecieveInstruction();
            _user2.AddRecieveInstruction();
        }
    }

    private void _user1_Finished(object? sender, UserHubChatArgs e)
    {
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
    }

    public override string ScenarioName => "PingChatHub";

    public override string Description => "Testing connectivity of th hub";

    private async Task PingHub()
    {
        await Connection.StartAsync();

        //await Connection.SendAsync(Hub_Send_Message_Topic, "xxx", "yyyy");

        //await _user1.SendAsync("yyy");


        //await _signal.WaitAsync(TimeSpan.FromSeconds(20));

        await Task.Delay(TimeSpan.FromMinutes(20));
    }

}
