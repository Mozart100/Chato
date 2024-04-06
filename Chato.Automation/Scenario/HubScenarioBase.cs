using Chato.Automation.Infrastructure;
using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chato.Automation.Scenario;

public interface IHubConnector
{
    Task SendMessageToAllUSers(string userNameFrom, string message);
    IAutomationLogger Logger { get; }
}

public abstract class HubScenarioBase : ScenarioBase, IHubConnector
{
    protected const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);

    protected Dictionary<string, UserHubChat> Users;
   
    protected HubConnection Connection;
    protected SemaphoreSlim Signal;



    public HubScenarioBase(string baseUrl) : base(baseUrl)
    {
        Users = new Dictionary<string, UserHubChat>();
        Signal = new SemaphoreSlim(0,1);


        Connection = new HubConnectionBuilder()
         .WithUrl(BaseUrl)
         .WithAutomaticReconnect()
         .Build();

        Connection.Reconnecting += (sender) =>
        {
            Logger.Info("Connection reconnecting");
            return Task.CompletedTask;
        };

        Connection.Reconnected += (sender) =>
        {
            Logger.Info($"Hub Connected.");
            return Task.CompletedTask;

        };

        //Listen();
    }

    protected async Task Listen()
    {
        Connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, async (user, message) =>
        {
            foreach (var keyAndValue in Users)
            {
                if (user.Equals(keyAndValue.Key, StringComparison.OrdinalIgnoreCase) == false)
                {
                    var userHub = keyAndValue.Value;
                    userHub.Received(user, message);
                }
            }

            if  (Users.Values.All(x => x.IsSuccessful))
            {
                await Connection.StopAsync();
                Signal.Release();
            }
        });

        await Signal.WaitAsync();
    }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        await Connection.SendAsync(Hub_Send_Message_Topic, userNameFrom, message);
    }
}
