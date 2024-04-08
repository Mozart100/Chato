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

public record HubMessageRecieved(string From, string Message);


public abstract class HubScenarioBase : ScenarioBase, IHubConnector
{

    protected const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);

    protected Dictionary<string, UserHubChat> Users;

    protected HubConnection Connection;
    protected SemaphoreSlim Signal;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Queue<HubMessageRecieved> _receivedMessages;

    public HubScenarioBase(string baseUrl) : base(baseUrl)
    {
        Users = new Dictionary<string, UserHubChat>();
        Signal = new SemaphoreSlim(0, 1);

        _cancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () => await ListeningThread(_cancellationTokenSource.Token));


        _receivedMessages = new Queue<HubMessageRecieved>();

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
    }

    private async Task ListeningThread(CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            if (_receivedMessages.TryDequeue(out var message) == false)
            {
                await Task.Delay(1000);
                continue;
            }

            foreach (var keyAndValue in Users)
            {
                if (message.From.Equals(keyAndValue.Key, StringComparison.OrdinalIgnoreCase) == false)
                {
                    var userHub = keyAndValue.Value;
                    userHub.Received(message.From, message.Message);
                }
            }

            if (Users.Values.All(x => x.IsSuccessful))
            {
                _cancellationTokenSource.Cancel();
                await Connection.StopAsync();
                Signal.Release();
            }
        }
    }

    protected async Task Listen()
    {
        Connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, async (user, message) =>
        {
            _receivedMessages.Enqueue(new HubMessageRecieved(user, message));
        });
    }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        await Connection.SendAsync(Hub_Send_Message_Topic, userNameFrom, message);
    }
}
