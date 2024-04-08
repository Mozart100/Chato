﻿using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;
using System.Collections.Generic;

namespace Arkovean.Chat.Automation.Scenario;

internal class PingChatScenario : HubScenarioBase
{
    private UserHubChat _user1;
    private UserHubChat _user2;


    public PingChatScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(UserSetups);
        BusinessLogicCallbacks.Add(ListenStep);
        SummaryLogicCallback.Add(Cleannup);
    }

  
    public override string ScenarioName => "PingChatHub";
    public override string Description => "Testing connectivity of the hub";

    private async Task UserSetups()
    {
        _user1 = new UserHubChat(this, "anatoliy");
        _user2 = new UserHubChat(this, "nathan");


        Users.Add(_user1.Name, _user1);
        Users.Add(_user2.Name, _user2);


        for (int i = 0; i < 5; i++)
        {
            _user1.AddRecieveInstruction();
            _user2.AddRecieveInstruction();
        }

        await Task.Delay(5 * 1000);
        await Connection.StartAsync();
    }


    private async Task ListenStep()
    {

        await Listen();
        //await Connection.SendAsync(Hub_Send_Message_Topic, "xxx", "yyyy");

        //await _user1.SendAsync("yyy");


        //await _signal.WaitAsync(TimeSpan.FromSeconds(20));

        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
        Logger.Info("Fiinished");
    }

    private async Task Cleannup()
    {
        if (Connection.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Disconnected)
        {
            await Connection.StopAsync();
        }
    }

}
