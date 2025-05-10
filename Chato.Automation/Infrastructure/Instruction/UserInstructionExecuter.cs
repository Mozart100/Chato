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
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Linq.Expressions;


namespace Chato.Automation.Infrastructure.Instruction;


public record HubMessageByteRecieved(string ChatNAme, string From, string Data, string? ImagePath);


public class UserInstructionExecuter
{
    public const string Hub_From_Server = "server";

    private const string Hub_Send_Other_In_Group_Topic = nameof(ChattoHub.SendMessageToOthersInChat);
    private const string Hub_Leave_Group_Topic = nameof(ChattoHub.LeaveGroup);
    private const string Hub_Join_Group_Topic = nameof(ChattoHub.JoinOrCreateChat);
    private const string Hub_History_Topic = nameof(ChattoHub.DownloadHistory);

    private const string Hub_RemoveGroupHistory_Topic = nameof(ChattoHub.RemoveChatHistory);
    private const string Hub_OnDisconnectedAsync_Topic = nameof(ChattoHub.UserDisconnectAsync);


    private readonly ILogger _logger;
    private readonly CounterSignal _counterSignal;
    private readonly HubConnection _connection;
    private readonly Queue<HubMessageByteRecieved> _receivedMessages;


    public UserInstructionExecuter(RegistrationResponse registerResponse, string url, ILogger logger, CounterSignal counterSignal)
    {

        _logger = logger;
        this._counterSignal = counterSignal;

        _receivedMessages = new Queue<HubMessageByteRecieved>();

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

        await JoinOrCreateChat(IChatService.Lobi, ChatType.Public, null);
        await DownloadHistory(IChatService.Lobi, amountMessages);
    }

    public RegistrationResponse RegisterResponse { get; }

    public string UserName => RegisterResponse.UserName;

    public async Task SendMessageToOthersInGroup(string chatName, string userNameFrom, string message, SenderInfoType messageType, string? imageName, string? description)
    {
        //var message = Encoding.UTF8.GetString(ptr);
        _logger.LogInformation($"{userNameFrom} sending in group [{chatName}] message [{message}].");

        if (messageType != SenderInfoType.TextMessage)
        {

        }
        //var ptr = Encoding.UTF8.GetBytes(message);
        await _connection.InvokeAsync(Hub_Send_Other_In_Group_Topic, new MessageInfo(messageType, chatName, userNameFrom, message, imageName));
    }


    public async Task JoinOrCreateChat(string chatName, ChatType chatType, string? description)
    {
        _logger.LogInformation($"{UserName} joins or create a chat.");

        await _connection.InvokeAsync(Hub_Join_Group_Topic, chatName, chatType, description);
    }

    public async Task DownloadHistory(string chatName, int amountMessages = -1)
    {
        _logger.LogInformation($"{UserName} get history of the chat.");


        var isToVerify = amountMessages > 0;
        await foreach (var senderInfo in _connection.StreamAsync<MessageInfo>(Hub_History_Topic, chatName))
        {
            var json = JsonSerializer.Serialize(senderInfo);
            _logger.LogInformation($"Downloading message: [{json}] in chat [{chatName}]");
            amountMessages--;

            if (senderInfo.SenderInfoType == SenderInfoType.Image)
            {
                senderInfo.TextMessage.Should().BeNullOrEmpty();
                senderInfo.Image.IsNotEmpty().Should().BeTrue();
                senderInfo.Image.Should().Contain($"{IChatService.ChatImages}");
            }
        }

        if (isToVerify)
        {
            amountMessages.Should().Be(0);
        }

        await _counterSignal.ReleaseAsync();

    }

    public async Task LeaveChatInfo(string groupName)
    {
        _logger.LogInformation($"Getting {UserName} info.");

        await _connection.InvokeAsync(Hub_Leave_Group_Topic, groupName);
    }

    public async Task MessageShouldBe(string chatName, string user, string fromArrived, string message, string imagePath)
    {
        try
        {
            _logger.LogWarning($"In {chatName} -- From user {user} hould be [{message}].");

            var messageReceived = _receivedMessages.Dequeue();

            if (messageReceived is HubMessageByteRecieved stringMessage)
            {
                stringMessage.From.Should().Be(fromArrived);
                stringMessage.Data.Should().Be(message);
                stringMessage.ChatNAme.Should().Be(chatName);

                if (stringMessage.ImagePath.IsNullOrEmpty() == false)
                {

                }
                stringMessage.ImagePath.Should().Be(imagePath);
            }
        }
        catch (Exception exception)
        {
            throw;
        }
    }

    public async Task NotReceivedCheckAsync()
    {
        _receivedMessages.Any().Should().BeFalse();
        await _counterSignal.ReleaseAsync();
    }


    protected async Task ListenAsync()
    {
        //_connection.On<string, string>(nameof(IChatHub.SendText), async (user, message) =>
        //{
        //    var ptr = Encoding.UTF8.GetBytes(message);
        //    await ExpectedMessagesAsync(IChatService.Lobi, user, message, null);
        //    await _counterSignal.ReleaseAsync();
        //});


        _connection.On<MessageInfo>(nameof(IChatHub.SendTextToChat), async (messageInfo) =>
        {

            if (messageInfo.SenderInfoType == SenderInfoType.Image || messageInfo.Image.IsNotEmpty())
            {

            }
            try
            {
                await ExpectedMessagesAsync(messageInfo.ChatName, messageInfo.FromUser, messageInfo.TextMessage, messageInfo.Image);
            }
            catch (Exception e)
            {
                _logger.LogWarning("xxxxxx");
            }

            await _counterSignal.ReleaseAsync();
        });


    }

    private async Task ExpectedMessagesAsync(string chatName, string fromUser, string message, string imagePath)
    {
        _logger.LogWarning($"In {chatName} -- From user {fromUser}  received message [{message}].");

        _receivedMessages.Enqueue(new HubMessageByteRecieved(chatName, fromUser, message, imagePath));
    }


    public async Task UserDisconnectingAsync()
    {
        await _connection.InvokeAsync(Hub_OnDisconnectedAsync_Topic);
        await _counterSignal.ReleaseAsync();
    }

    public async Task KillConnectionAsync()
    {
        await _connection.StopAsync();
    }
}
