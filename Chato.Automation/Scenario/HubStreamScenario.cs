﻿using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;

namespace Arkovean.Chat.Automation.Scenario;

internal class HubStreamScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    public HubStreamScenario(string baseUrl) : base(baseUrl)
    {

        SetupsLogicCallback.Add(TwoUserSetups);
        BusinessLogicCallbacks.Add(TwoPeopleHandShakeStep);
        SummaryLogicCallback.Add(GroupUsersCleanup);

    }


    public override string ScenarioName => "Streaming messages";

    public override string Description => "Uploading and downloading messaging.";

    private async Task TwoUserSetups()
    {
        await InitializeWithGroupAsync(First_Group, Anatoliy_User, Olessya_User);
    }

    private async Task TwoPeopleHandShakeStep()
    {
        var message_1 = "Hello";
        var groupRoot = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);

        var anatoliySender = groupRoot.IsToDownload(Anatoliy_User);
        var olessyaReceive = groupRoot.IsToDownload(Olessya_User);


        anatoliySender.Connect(olessyaReceive);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }

    public async Task GroupUsersCleanup()
    {
        await GroupUsersCleanup(First_Group);
    }

}
