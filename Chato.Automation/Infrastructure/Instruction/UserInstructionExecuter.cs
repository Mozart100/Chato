using Chato.Automation.Extensions;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chatto.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Chato.Automation.Infrastructure.Instruction;


public record HubMessageByteRecieved2222(string ChatNAme, string From, string Data);


public class UserInstructionExecuter
{
    public const string Hub_From_Server = "server";

    //private const string Hub_Send_Message_To_Others_Topic = nameof(ChattoHub.BroadcastMessage);

    private const string Hub_Send_Other_In_Group_Topic = nameof(ChattoHub.SendMessageToOthersInGroup);
    private const string Hub_Leave_Group_Topic = nameof(ChattoHub.LeaveGroup);
    private const string Hub_Join_Group_Topic = nameof(ChattoHub.JoinOrCreateChat);
    private const string Hub_History_Topic = nameof(ChattoHub.DownloadHistory);
    //private const string Hub_Join_Group_Topic = nameof(ChattoHub.JoinOrCreateGroup);

    private const string Hub_Download_Topic = nameof(ChattoHub.Downloads);
    private const string Hub_GetGroupHistory_Topic = nameof(ChattoHub.GetGroupHistory);
    private const string Hub_RemoveGroupHistory_Topic = nameof(ChattoHub.RemoveChatHistory);
    private const string Hub_OnDisconnectedAsync_Topic = nameof(ChattoHub.UserDisconnectAsync);



    private readonly ILogger _logger;
    private readonly CounterSignal _signal;
    private readonly HubConnection _connection;
    //private readonly Queue<HubMessageRecievedBase> _receivedMessages;
    private readonly Queue<HubMessageByteRecieved2222> _receivedMessages;
    private readonly HashSet<string> _ignoreUsers;


    public UserInstructionExecuter(RegistrationResponse registerResponse, string url, ILogger logger, CounterSignal signal)
    {

        _logger = logger;
        this._signal = signal;
        _ignoreUsers = new HashSet<string>();
        _ignoreUsers.Add(Hub_From_Server);

        //_receivedMessages = new Queue<HubMessageRecievedBase>();
        _receivedMessages = new Queue<HubMessageByteRecieved2222>();

        _connection = new HubConnectionBuilder()
       .WithUrl(url, options =>
       {
           options.AccessTokenProvider = async () => RegisterResponse.Token;
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
        //LoginResponse = loginResponse;
    }


    public async Task StartSignalRAsync(int amountMessages)
    {
        await _connection.StartAsync();
        await ListenAsync();

        await JoinOrCreateChat(IChatService.Lobi);
        await DownloadHistory(IChatService.Lobi, amountMessages);
    }

    public RegistrationResponse RegisterResponse { get; }

    public string UserName => RegisterResponse.UserName;

    public async Task SendMessageToOthersInGroup(string chatName, string userNameFrom, byte[] ptr)
    {
        var message = Encoding.UTF8.GetString(ptr);
        _logger.LogInformation($"{userNameFrom} sending in group [{chatName}] message [{message}].");


        //var ptr = Encoding.UTF8.GetBytes(message);
        await _connection.InvokeAsync(Hub_Send_Other_In_Group_Topic, chatName, userNameFrom, message);
    }


    public async Task JoinOrCreateChat(string chatName)
    {
        _logger.LogInformation($"{UserName} joins or create a chat.");

        await _connection.InvokeAsync(Hub_Join_Group_Topic, chatName);


        //var isToVerify = amountMessages > 0;
        //await foreach (var senderInfo in _connection.StreamAsync<HistsoryMessageInfo>(Hub_Join_Group_Topic, chatName))
        //{
        //    amountMessages--;
        //}

        //if (isToVerify)
        //{
        //    amountMessages.Should().Be(0);
        //}


        //await _signal.ReleaseAsync();
    }

    public async Task DownloadHistory(string chatName, int amountMessages = -1)
    {
        _logger.LogInformation($"{UserName} get history of the chat.");


        var isToVerify = amountMessages > 0;
        await foreach (var senderInfo in _connection.StreamAsync<HistoryMessageInfo>(Hub_History_Topic, chatName))
        {
            amountMessages--;
        }

        if (isToVerify)
        {
            amountMessages.Should().Be(0);
        }

        await _signal.ReleaseAsync();
    }



    //public async Task JoinOrCreateChat(string chatName)
    //{
    //    _logger.LogInformation($"{UserName} joins chat.");

    //    await _connection.InvokeAsync(Hub_Join_Group_Topic, chatName);
    //    await _signal.ReleaseAsync();
    //}


    public async Task LeaveChatInfo(string groupName)
    {
        _logger.LogInformation($"Getting {UserName} info.");

        await _connection.InvokeAsync(Hub_Leave_Group_Topic, groupName);
    }


    public async Task DownloadStream(byte[] message)
    {
        _logger.LogInformation($"{UserName} download.");

        await foreach (var item in _connection.StreamAsync<byte[]>(Hub_Download_Topic, new HubDownloadInfo(1)))
        {
            item.Count().Should().Be(message.Length);
        }
    }

    public async Task ReceivedMessage(string chatName, string user, string fromArrived, byte[] ptr)
    {
        var message = Encoding.UTF8.GetString(ptr);
        await ShouldBe(chatName, user, fromArrived, message);
    }

    public async Task ShouldBe(string chatName, string user, string fromArrived, string message)
    {
        _logger.LogWarning($"In {chatName} -- From user {user} hould be [{message}].");

        var messageReceived = _receivedMessages.Dequeue();

        if (messageReceived is HubMessageByteRecieved2222 stringMessage)
        {
            stringMessage.From.Should().Be(fromArrived);
            stringMessage.Data.Should().Be(message);
            stringMessage.ChatNAme.Should().Be(chatName);
        }
    }

    public async Task NotReceivedCheckAsync()
    {
        _receivedMessages.Any().Should().BeFalse();
        await _signal.ReleaseAsync();
    }


    protected async Task ListenAsync()
    {
        _connection.On<string, string>(nameof(IChatHub.SendText), async (user, message) =>
        {
            var ptr = Encoding.UTF8.GetBytes(message);
            await ExpectedMessagesAsync(IChatService.Lobi, user, message);
            await _signal.ReleaseAsync();
        });


        _connection.On<string, string, string>(nameof(IChatHub.SendTextToChat), async (chat, fromUser, message) =>
        {
            var ptr = Encoding.UTF8.GetBytes(message);
            await ExpectedMessagesAsync(chat, fromUser, message);
            await _signal.ReleaseAsync();
        });



    }

    private async Task ExpectedMessagesAsync(string chatName, string fromUser, string message)
    {
        _logger.LogWarning($"In {chatName} -- From user {fromUser}  received message [{message}].");

        _receivedMessages.Enqueue(new HubMessageByteRecieved2222(chatName, fromUser, message));
    }


    public async Task UserDisconnectingAsync()
    {
        await _connection.InvokeAsync(Hub_OnDisconnectedAsync_Topic);
        await _signal.ReleaseAsync();
    }

    public async Task KillConnectionAsync()
    {
        await _connection.StopAsync();
    }
}
