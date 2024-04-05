using Arkovean.Chat.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Arkovean.Chat.Automation.Scenario;

internal class PingChatScenario : ScenarioBase
{
    private HubConnection _connection;

    public PingChatScenario(string baseUrl) : base(baseUrl)
    {
        _connection = new HubConnectionBuilder()
         .WithUrl(BaseUrl)
         .WithAutomaticReconnect()
         .Build();


        if (_connection.State == HubConnectionState.Connected)
        {
            // Hub _connection is open
        }
        else if (_connection.State == HubConnectionState.Connecting)
        {
            // Hub _connection is in the process of connecting
        }
        else if (_connection.State == HubConnectionState.Disconnected)
        {
            // Hub _connection is closed
        }
        else if (_connection.State == HubConnectionState.Reconnecting)
        {
            // Hub connection is in the process of reconnecting
        }


        _connection.Reconnecting += (sender) =>
        {
            Logger.Info("Connection reconnecting");
            return Task.CompletedTask;
        };

        _connection.Reconnected += (sender) =>
        {
           Logger.Info($"Hub Connected.");
            return Task.CompletedTask;

        };

        BusinessLogicCallbacks.Add(PingHub);
    }


    public override string ScenarioName => "PingChatHub";

    public override string Description => "Testing connectivity of th hub";

    private async Task PingHub()
    {
        await _connection.StartAsync();

        _connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, (user, message) =>
        {
            Logger.Info($"{user} - {message}");
        });

        await Task.Delay(TimeSpan.FromMinutes(20));
    }

}
