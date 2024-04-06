using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class PingChatScenario : HubScenarioBase
{
    private readonly UserHubChat _user1;
    private readonly UserHubChat _user2;

    private SemaphoreSlim _signal;

    public PingChatScenario(string baseUrl) : base(baseUrl)
    {
        _user1 = new UserHubChat(this, "anatoliy");
        _user2 = new UserHubChat(this, "nathan");

        Users.Add(_user1.Name, _user1);
        Users.Add(_user2.Name, _user2);



        _signal = new SemaphoreSlim(1);

        BusinessLogicCallbacks.Add(PingHub);
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
