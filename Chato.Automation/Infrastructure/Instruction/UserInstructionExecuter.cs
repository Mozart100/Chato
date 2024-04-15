using Chato.Automation.Infrastructure;
using Chato.Server.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chato.Automation.Infrastructure.Instruction;

public record HubMessageRecieved(string From, string Message);


public class UserInstructionExecuter
{

    private const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);

    private readonly IAutomationLogger _logger;
    private readonly HubConnection _connection;
    private readonly Queue<HubMessageRecieved> _receivedMessages;
    private readonly HashSet<string> _ignoreUsers;


    public UserInstructionExecuter(string userName, string url, IAutomationLogger logger)
    {
        UserName = userName;
        _logger = logger;

        _ignoreUsers = new HashSet<string>();
        _ignoreUsers.Add("server");

        _receivedMessages = new Queue<HubMessageRecieved>();

        _connection = new HubConnectionBuilder()
       .WithUrl(url)
       .WithAutomaticReconnect()
       .Build();

        _connection.Reconnecting += (sender) =>
        {
            _logger.Info("Connection reconnecting");
            return Task.CompletedTask;
        };

        _connection.Reconnected += (sender) =>
        {
            _logger.Info($"Hub Connected.");
            return Task.CompletedTask;

        };
    }

    public async Task InitializeAsync()
    {
        await _connection.StartAsync();
        await Listen();
    }

    public string UserName { get; }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        _logger.Info($"{userNameFrom} sening message [{message}].");

        await _connection.SendAsync(Hub_Send_Message_Topic, userNameFrom, message);
    }

    public async Task ListenCheck(string fromArrived, string message)
    {
        var messageReceived = _receivedMessages.Dequeue();

        messageReceived.From.Should().Be(fromArrived);
        messageReceived.Message.Should().Be(message);
    }

    protected async Task Listen()
    {
        _connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, async (user, message) =>
        {
            if (_ignoreUsers.Contains(user) == false)
            {
                _logger.Info($"{UserName} received message [{message}] from [{user}].");

                _receivedMessages.Enqueue(new HubMessageRecieved(user, message));
            }
            else
            {
                _logger.Info($"{UserName} received message [{message}] but was ignored from [{user}].");
            }
        });
    }

    public async Task Close()
    {
        await _connection.StopAsync();
    }
}
