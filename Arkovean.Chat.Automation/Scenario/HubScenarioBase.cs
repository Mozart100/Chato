﻿using Arkovean.Chat.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Arkovean.Chat.Automation.Scenario;

public abstract class HubScenarioBase : ScenarioBase
{
    protected const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);

    protected Dictionary<string, UserHubChat> Users;
    protected HubConnection Connection;

    public HubScenarioBase(string baseUrl) : base(baseUrl)
    {

        Users = new Dictionary<string, UserHubChat>();

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

        Listen();
    }

    private void Listen()
    {
        Connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, (user, message) =>
        {
            foreach (var keyAndValue in Users)
            {
                if (user.Equals(keyAndValue.Key, StringComparison.OrdinalIgnoreCase) == false)
                {
                    keyAndValue.Value.Received(user,message);
                }
            }
        });
    }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        await Connection.SendAsync(Hub_Send_Message_Topic, userNameFrom, message);
    }


}