using Chato.Server.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Text;

namespace Chato.Automation.Infrastructure.Instruction;

public record HubMessageRecievedBase(string From);
public record HubMessageStringRecieved(string From, string Message) : HubMessageRecievedBase(From);
public record HubMessageByteRecieved(string From, byte [] data) : HubMessageRecievedBase(From);


public class UserInstructionExecuter
{
    public const string Hub_From_Server = "server";

    private const string Hub_Send_Message_To_Others_Topic = nameof(ChatHub.SendMessageToOthers);

    private const string Hub_Send_Other_In_Group_Topic = nameof(ChatHub.SendMessageToOthersInGroup);
    private const string Hub_Leave_Group_Topic = nameof(ChatHub.LeaveGroup);
    private const string Hub_Join_Group_Topic = nameof(ChatHub.JoinGroup);

    private const string Hub_Download_Topic = nameof(ChatHub.Download);



    private readonly IAutomationLogger _logger;
    private readonly CounterSignal _signal;
    private readonly HubConnection _connection;
    private readonly Queue<HubMessageRecievedBase> _receivedMessages;
    private readonly HashSet<string> _ignoreUsers;


    public UserInstructionExecuter(string userName, string url, IAutomationLogger logger, CounterSignal signal)
    {
        UserName = userName;
        _logger = logger;
        this._signal = signal;
        _ignoreUsers = new HashSet<string>();
        _ignoreUsers.Add(Hub_From_Server);

        _receivedMessages = new Queue<HubMessageRecievedBase>();

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

    public async Task InitializeWithGroupAsync(string groupName)
    {
        await _connection.StartAsync();
        await JoinGroup(groupName);

        await Listen();
    }

    public string UserName { get; }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        _logger.Info($"{userNameFrom} sening message [{message}].");

        await _connection.SendAsync(Hub_Send_Message_To_Others_Topic, userNameFrom, message);
    }

    public async Task SendMessageToOthersInGroup(string groupName, string userNameFrom, string message)
    {
        _logger.Info($"{userNameFrom} sening in group [{groupName}] message [{message}].");

        await _connection.InvokeAsync(Hub_Send_Other_In_Group_Topic, groupName, userNameFrom, message);
    }


    public async Task JoinGroup(string groupName)
    {
        _logger.Info($"{UserName} joins group.");

        await _connection.InvokeAsync(Hub_Join_Group_Topic, groupName);
    }


    public async Task DownloadStream(byte[] message)
    {
        _logger.Info($"{UserName} download.");

        await foreach (var item in _connection.StreamAsync<string>(Hub_Download_Topic, new HubDownloadInfo(10)))
        {
            message.Count().Should().Be(message.Length);    
        }
    }

    public async Task ListenStringCheck(string fromArrived, byte []  ptr)
    {
        var message = Encoding.UTF8.GetString(ptr);
        var messageReceived = _receivedMessages.Dequeue();

        if (messageReceived is HubMessageStringRecieved stringMessage)
        {
            stringMessage.From.Should().Be(fromArrived);
            stringMessage.Message.Should().Be(message);
        }
    }


    public async Task NotReceivedCheck()
    {
        _receivedMessages.Any().Should().BeFalse();
    }

    protected async Task Listen()
    {
        _connection.On<string, string>(ChatHub.TOPIC_MESSAGE_STRING_RECEIVED, async (user, message) =>
        {
            await ExpectedStringMessages(user, message);
        });
    }

    private async Task ExpectedStringMessages(string user, string message)
    {
        if (_ignoreUsers.Contains(user) == false)
        {
            _logger.Info($"{UserName} received message [{message}] from [{user}].");

            _receivedMessages.Enqueue(new HubMessageStringRecieved(user, message));
            await _signal.ReleaseAsync();
        }
        else
        {
            _logger.Info($"{UserName} received message [{message}] but was ignored from [{user}].");
        }
    }

    public async Task Close()
    {
        await _connection.StopAsync();
    }

    public async Task GroupClose(string groupName)
    {
        await _connection.InvokeAsync(Hub_Leave_Group_Topic, groupName);
        await _connection.StopAsync();
    }
}
