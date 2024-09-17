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

public record HubMessageRecievedBase(string From);
//public record HubMessageStringRecieved(string From, string Message) : HubMessageRecievedBase(From);
public record HubMessageByteRecieved(string From, string Data) : HubMessageRecievedBase(From);
public record HubMessageByteRecieved2222(string ChatNAme, string From, string Data);


public class UserInstructionExecuter
{
    public const string Hub_From_Server = "server";

    //private const string Hub_Send_Message_To_Others_Topic = nameof(ChattoHub.BroadcastMessage);

    private const string Hub_Send_Other_In_Group_Topic = nameof(ChattoHub.SendMessageToOthersInGroup);
    private const string Hub_Leave_Group_Topic = nameof(ChattoHub.LeaveGroup);
    private const string Hub_Join_Group_Topic = nameof(ChattoHub.JoinOrCreateGroup);
    //private const string Hub_Get_Group_Info_Topic = nameof(ChattoHub.GetGroupInfo);

    private const string Hub_Download_Topic = nameof(ChattoHub.Downloads);
    private const string Hub_GetGroupHistory_Topic = nameof(ChattoHub.GetGroupHistory);
    private const string Hub_RemoveGroupHistory_Topic = nameof(ChattoHub.RemoveChatHistory);
    private const string Hub_SendMessageToOtherUser_Topic = nameof(ChattoHub.SendMessageToOtherUser);
    private const string Hub_OnDisconnectedAsync_Topic = nameof(ChattoHub.UserDisconnectAsync);



    private readonly ILogger _logger;
    private readonly CounterSignal _signal;
    private readonly HubConnection _connection;
    //private readonly Queue<HubMessageRecievedBase> _receivedMessages;
    private readonly Queue<HubMessageByteRecieved2222> _receivedMessages2222;
    private readonly HashSet<string> _ignoreUsers;


    public UserInstructionExecuter(RegistrationResponse registerResponse, string url, ILogger logger, CounterSignal signal)
    {

        _logger = logger;
        this._signal = signal;
        _ignoreUsers = new HashSet<string>();
        _ignoreUsers.Add(Hub_From_Server);

        //_receivedMessages = new Queue<HubMessageRecievedBase>();
        _receivedMessages2222 = new Queue<HubMessageByteRecieved2222>();

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

    
    public async Task RegisterAsync2222()
    {
        await _connection.StartAsync();
        await Listen2222();


        //var ptr = Encoding.UTF8.GetBytes(ChattoHub.User_Connected_Message);
        //ExpectedMessages(UserName, ptr);
    }

    public async Task InitializeWithGroupAsync(string groupName)
    {
        await JoinOrCreateChat2222(groupName);
        await DownloadGroupHistory(groupName);
    }

    public RegistrationResponse RegisterResponse { get; }

    public string UserName => RegisterResponse.UserName;

    public async Task DownloadGroupHistory(string groupName)
    {
        _logger.LogInformation($"{UserName} downloading history of the group.");

        await foreach (var senderInfo in _connection.StreamAsync<SenderInfo>(Hub_GetGroupHistory_Topic, groupName))
        {
            await ExpectedMessages2222( groupName, senderInfo.UserName, senderInfo.Message);
        }
    }

    //public async Task SendMessageToAllUsers(string userNameFrom, byte[] message)
    //{
    //    _logger.LogInformation($"{userNameFrom} sending message [{message}].");

    //    await _connection.SendAsync(Hub_Send_Message_To_Others_Topic, userNameFrom, message);
    //}

    public async Task SendMessageToOtherUser(string userNameFrom, string toUser, byte[] ptr)
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
        await _connection.InvokeAsync(Hub_Send_Other_In_Group_Topic, groupName, userNameFrom, UserName, templateMessage);
    }


    public async Task JoinOrCreateChat2222(string groupName)
    {
        _logger.LogInformation($"{UserName} joins group.");

        await _connection.InvokeAsync(Hub_Join_Group_Topic, groupName);
        await _signal.ReleaseAsync();
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

    public async Task ListenToStringCheckAsync2222(string chatName, string user , string fromArrived, byte[] ptr)
    {
        try
        {


            var message = Encoding.UTF8.GetString(ptr);
            await ListenToStringCheckAsync2222(chatName,user, fromArrived, message);
        }
        catch (Exception ex)
        {

        }
    }

    public async Task ListenToStringCheckAsync2222(string chatName, string user, string fromArrived, string message)
    {
        _logger.LogWarning($"In {chatName} -- From user {user} hould be [{message}].");

        var messageReceived = _receivedMessages2222.Dequeue();

        if (messageReceived is HubMessageByteRecieved2222 stringMessage)
        {
            stringMessage.From.Should().Be(fromArrived);
            stringMessage.Data.Should().Be(message);
            stringMessage.ChatNAme.Should().Be(chatName);
        }
    }


    //public async Task ListenToStringCheckAsync(string chatName, string fromArrived, byte[] ptr)
    //{
    //    var message = Encoding.UTF8.GetString(ptr);
    //    var messageReceived = _receivedMessages2222.Dequeue();

    //    if (messageReceived is HubMessageByteRecieved2222 stringMessage)
    //    {
    //        stringMessage.From.Should().Be(fromArrived);
    //        stringMessage.Data.Should().Be(message);
    //        stringMessage.ChatNAme.Should().Be(chatName);
    //    }
    //}

    public async Task NotReceivedCheckAsync2222()
    {
        _receivedMessages2222.Any().Should().BeFalse();
        await _signal.ReleaseAsync();
    }


    protected async Task Listen2222()
    {
        _connection.On<string, string>(nameof(IChatHub.SendText), async (user, message) =>
        {
            var ptr = Encoding.UTF8.GetBytes(message);
            await ExpectedMessages2222(IChatService.Lobi, user, message);
            await _signal.ReleaseAsync();
        });


        //_connection.On<string>(nameof(IChatHub.SelfReplay), async (message) =>
        //{
        //    var ptr = Encoding.UTF8.GetBytes(message);
        //    await ExpectedMessages(UserName, ptr);
        //});


        _connection.On<string, string, string, string>(nameof(IChatHub.SendTextToChat), async (chat, fromUser, toUser, message) =>
        {
            var ptr = Encoding.UTF8.GetBytes(message);
            await ExpectedMessages2222(chat, fromUser, message);
            await _signal.ReleaseAsync();

            //_lokkcgger.LogWarning($"{nameof(IChatHub.SendTextToChat)} -- From user {fromUser} in chat {chat} received message [{message}].");
        });



    }

    private async Task ExpectedMessages2222(string chatName, string fromUser, string message)
    {
        _logger.LogWarning($"In {chatName} -- From user {fromUser}  received message [{message}].");

        if(UserName.Equals(fromUser, StringComparison.OrdinalIgnoreCase))
        {

        }


        _receivedMessages2222.Enqueue(new HubMessageByteRecieved2222(chatName, fromUser, message));
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
