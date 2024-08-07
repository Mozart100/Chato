﻿using Chato.Automation.Extensions;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Infrastracture;
using Chatto.Shared;
using FluentAssertions;
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

    private const string Hub_Send_Message_To_Others_Topic = nameof(ChattoHub.BroadcastMessage);

    private const string Hub_Send_Other_In_Group_Topic = nameof(ChattoHub.SendMessageToOthersInGroup);
    private const string Hub_Leave_Group_Topic = nameof(ChattoHub.LeaveGroup);
    private const string Hub_Join_Group_Topic = nameof(ChattoHub.JoinGroup);
    //private const string Hub_Get_Group_Info_Topic = nameof(ChattoHub.GetGroupInfo);

    private const string Hub_Download_Topic = nameof(ChattoHub.Downloads);
    private const string Hub_GetGroupHistory_Topic = nameof(ChattoHub.GetGroupHistory);
    private const string Hub_RemoveGroupHistory_Topic = nameof(ChattoHub.RemoveChatHistory);
    private const string Hub_SendMessageToOtherUser_Topic = nameof(ChattoHub.SendMessageToOtherUser);
    private const string Hub_OnDisconnectedAsync_Topic = nameof(ChattoHub.UserDisconnectAsync);



    private readonly ILogger _logger;
    private readonly CounterSignal _signal;
    private readonly HubConnection _connection;
    private readonly Queue<HubMessageRecievedBase> _receivedMessages;
    private readonly HashSet<string> _ignoreUsers;


    public UserInstructionExecuter(RegistrationResponse registerResponse, string url, ILogger logger, CounterSignal signal)
    {

        _logger = logger;
        this._signal = signal;
        _ignoreUsers = new HashSet<string>();
        _ignoreUsers.Add(Hub_From_Server);

        _receivedMessages = new Queue<HubMessageRecievedBase>();

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

    public RegistrationResponse RegisterResponse { get; }

    public string UserName => RegisterResponse.UserName;

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

    public async Task SendMessageFromUserToUserUsers(string userNameFrom, string toUser, byte[] ptr)
    {
        var message = Encoding.UTF8.GetString(ptr);
        _logger.LogInformation($"{userNameFrom} sending message to user [{toUser}] this message [{message}].");

        await _connection.SendAsync(Hub_SendMessageToOtherUser_Topic, userNameFrom, toUser, message);
    }


    public async Task SendMessageToOthersInGroup(string groupName, string userNameFrom, byte[] ptr)
    {
        var isStringFile = FileHelper.IsFileText(ptr);
        var templateMessage = "image";

        if (isStringFile)
        {
            templateMessage = Encoding.UTF8.GetString(ptr);
        }

        _logger.LogInformation($"{userNameFrom} sending in group [{groupName}] message [{templateMessage}].");


        //var ptr = Encoding.UTF8.GetBytes(message);
        await _connection.InvokeAsync(Hub_Send_Other_In_Group_Topic, groupName, userNameFrom, templateMessage);
    }


    public async Task JoinGroup(string groupName)
    {
        _logger.LogInformation($"{UserName} joins group.");

        await _connection.InvokeAsync(Hub_Join_Group_Topic, groupName);
    }


    public async Task LeaveGroupInfo(string groupName)
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
        _connection.On<string, string>(nameof(IChatHub.SendText), async (user, message) =>
        {
            if (_ignoreUsers.Contains(user) == false)
            {
                var ptr = Encoding.UTF8.GetBytes(message);
                await ExpectedMessages(user, ptr);
            }
            else
            {
                //var message = Encoding.UTF8.GetString(message);
                _logger.LogWarning($"{UserName} received message [{message}] but was ignored from [{user}].");
            }
        });


        _connection.On<string>(nameof(IChatHub.SelfReplay), async (message) =>
        {
            var ptr = Encoding.UTF8.GetBytes(message);
            await ExpectedMessages(UserName, ptr);
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


    public async Task UserDisconnectingAsync()
    {
        await _connection.InvokeAsync(Hub_OnDisconnectedAsync_Topic);
    }

    public async Task KillConnectionAsync()
    {
        await _connection.StopAsync();
    }
}
