using Chato.Automation.Extensions;
using Chato.Automation.Responses;
using Chato.Server;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Infrastracture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Chato.Automation.Infrastructure.Instruction;

public record HubMessageRecievedBase(string From);
//public record HubMessageStringRecieved(string From, string Message) : HubMessageRecievedBase(From);
public record HubMessageByteRecieved(string From, byte[] Data) : HubMessageRecievedBase(From);


public class UserInstructionExecuter
{
    public const string Hub_From_Server = "server";

    private const string Hub_Send_Message_To_Others_Topic = nameof(ChatHub.SendMessageToOthers);

    private const string Hub_Send_Other_In_Group_Topic = nameof(ChatHub.SendMessageToOthersInGroup);
    private const string Hub_Leave_Group_Topic = nameof(ChatHub.LeaveGroup);
    private const string Hub_Join_Group_Topic = nameof(ChatHub.JoinGroup);

    private const string Hub_Download_Topic = nameof(ChatHub.Downloads);
    private const string Hub_GetGroupHistory_Topic = nameof(ChatHub.GetGroupHistory);
    private const string Hub_RemoveGroupHistory_Topic = nameof(ChatHub.RemoveChatHistory);



    private readonly ILogger _logger;
    private readonly CounterSignal _signal;
    private readonly HubConnection _connection;
    private readonly Queue<HubMessageRecievedBase> _receivedMessages;
    private readonly HashSet<string> _ignoreUsers;


    public UserInstructionExecuter(RegisterResponse registerResponse, LoginResponse loginResponse, string url, ILogger logger, CounterSignal signal)
    {

        _logger = logger;
        this._signal = signal;
        _ignoreUsers = new HashSet<string>();
        _ignoreUsers.Add(Hub_From_Server);

        _receivedMessages = new Queue<HubMessageRecievedBase>();

        _connection = new HubConnectionBuilder()
       .WithUrl(url, options =>
       {
           options.AccessTokenProvider = async () => LoginResponse.Token;
       })
       .WithAutomaticReconnect()
       .Build();

        _connection.Reconnecting += (sender) =>
        {
            _logger.LogInformation("Connection reconnecting");
            return Task.CompletedTask;
        };

        _connection.Reconnected += (sender) =>
        {
            _logger.LogInformation($"Hub Connected.");
            return Task.CompletedTask;

        };

        RegisterResponse = registerResponse;
        LoginResponse = loginResponse;
    }

    public async Task InitializeAsync()
    {
        await _connection.StartAsync();
        await Listen();
    }

    public async Task RegisterAsync()
    {
        await _connection.StartAsync();
        await Listen();
    }

    public async Task InitializeWithGroupAsync(string groupName)
    {
        await JoinGroup(groupName);
        await DownloadGroupHistory(groupName);
    }

    public RegisterResponse RegisterResponse { get; }
    public LoginResponse LoginResponse { get; }

    public string UserName => RegisterResponse.Username;

    public async Task DownloadGroupHistory(string groupName)
    {
        _logger.LogInformation($"{UserName} downloading history of the group.");

        await foreach (var senderInfo in _connection.StreamAsync<SenderInfo>(Hub_GetGroupHistory_Topic, groupName))
        {
            await ExpectedMessages(senderInfo.UserName, senderInfo.Message);
        }
    }

    public async Task SendMessageToAllUsers(string userNameFrom, byte[] message)
    {
        _logger.LogInformation($"{userNameFrom} sending message [{message}].");

        await _connection.SendAsync(Hub_Send_Message_To_Others_Topic, userNameFrom, message);
    }

    public async Task SendMessageFromUserToUserUsers(string userNameFrom, string toUser, byte[] message)
    {
        _logger.LogInformation($"{userNameFrom} sending message to user [{toUser}] this message [{message}].");

        await _connection.SendAsync(Hub_Send_Message_To_Others_Topic, userNameFrom, message);
    }


    public async Task SendMessageToOthersInGroup(string groupName, string userNameFrom, byte[] message)
    {
        var isStringFile = FileHelper.IsFileText(message);
        var templateMessage = "image";

        if (isStringFile)
        {
            templateMessage = Encoding.UTF8.GetString(message);
        }

        _logger.LogInformation($"{userNameFrom} sending in group [{groupName}] message [{templateMessage}].");


        //var ptr = Encoding.UTF8.GetBytes(message);
        await _connection.InvokeAsync(Hub_Send_Other_In_Group_Topic, groupName, userNameFrom, message);
    }


    public async Task JoinGroup(string groupName)
    {
        _logger.LogInformation($"{UserName} joins group.");

        await _connection.InvokeAsync(Hub_Join_Group_Topic, groupName);
    }


    public async Task DownloadStream(byte[] message)
    {
        _logger.LogInformation($"{UserName} download.");

        await foreach (var item in _connection.StreamAsync<byte[]>(Hub_Download_Topic, new HubDownloadInfo(1)))
        {
            item.Count().Should().Be(message.Length);
        }
    }

    public async Task ListenToStringCheckAsync(string fromArrived, byte[] ptr)
    {
        var message = Encoding.UTF8.GetString(ptr);
        var messageReceived = _receivedMessages.Dequeue();

        if (messageReceived is HubMessageByteRecieved stringMessage)
        {
            stringMessage.From.Should().Be(fromArrived);
            stringMessage.Data.IsEqualsShould(ptr);
        }
    }


    public async Task NotReceivedCheckAsync()
    {
        _receivedMessages.Any().Should().BeFalse();
    }

    protected async Task Listen()
    {
        _connection.On<string, byte[]>(nameof(IChatHub.MessageRecieved), async (user, ptr) =>
        {
            if (_ignoreUsers.Contains(user) == false)
            {
                await ExpectedMessages(user, ptr);
            }
            else
            {
                var message = Encoding.UTF8.GetString(ptr);
                _logger.LogWarning($"{UserName} received message [{message}] but was ignored from [{user}].");
            }
        });

    }

    private async Task ExpectedMessages(string user, byte[] ptr)
    {
        var isStringFile = FileHelper.IsFileText(ptr);
        var templateMessage = string.Empty;

        if (isStringFile)
        {
            templateMessage = Encoding.UTF8.GetString(ptr);
        }

        if (isStringFile)
        {
            _logger.LogInformation($"{UserName} received message [{templateMessage}] from [{user}].");
        }
        else
        {
            _logger.LogInformation($"{UserName} received message image from [{user}].");
        }

        _receivedMessages.Enqueue(new HubMessageByteRecieved(user, ptr));
        await _signal.ReleaseAsync();
    }

    public async Task Close()
    {
        await _connection.StopAsync();
    }

    public async Task GroupClose(string groupName)
    {
        await _connection.InvokeAsync(Hub_Leave_Group_Topic, groupName);
        await _connection.InvokeAsync(Hub_RemoveGroupHistory_Topic, groupName);
    }

    public async Task KillConnection()
    {
        await _connection.StopAsync();
    }
}
