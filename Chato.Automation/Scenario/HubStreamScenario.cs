﻿using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Arkovean.Chat.Automation.Scenario;

internal class HubStreamScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";
    private const string Second_Group = "nesher";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";


    private byte[] _fileContent;

    public HubStreamScenario(ILogger<HubStreamScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        SetupsLogicCallback.Add(TreeUserSetups);
        BusinessLogicCallbacks.Add(TreePoepleHandShakeStep);
        SummaryLogicCallback.Add(() => GroupUsersCleanup(First_Group, Second_Group));

        var path = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "css.txt");
        _fileContent = File.ReadAllBytes(path);

    }

    public override string ScenarioName => "Streaming messages";

    public override string Description => "Uploading and downloading messaging.";

    private async Task TreeUserSetups()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Natali_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
        await AssignUserToGroupAsync(Second_Group, Natali_User);
    }

    private async Task TreePoepleHandShakeStep()
    {
        var message_1 = _fileContent;
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);
        var secondtGroup = InstructionNodeFluentApi.StartWithGroup(groupName: Second_Group, message_1);



        var anatoliySender = firstGroup.SendingBroadcast(Anatoliy_User);
        var olessyaReceive1 = firstGroup.RecievingFrom(Olessya_User, anatoliySender.UserName);
        var nathanReceive1 = firstGroup.RecievingFrom(Nathan_User, anatoliySender.UserName);
        var nataliRecevier = secondtGroup.Is_Not_Received(Natali_User);


        var message_2 = "Shalom to you too";
        var secondRoot = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_2);


        var olessyaSender = secondRoot.SendingBroadcast(Olessya_User);
        var anatoliyReceiver = secondRoot.RecievingFrom(Anatoliy_User, olessyaSender.UserName);
        var nathanReceiver2 = secondRoot.RecievingFrom(Nathan_User, olessyaSender.UserName);
        var maxReceiver1 = secondRoot.RecievingFrom(Max_User, olessyaSender.UserName);


        anatoliySender.Connect(nataliRecevier, nathanReceive1, olessyaReceive1)
            .Do(maxReceiver1, async user =>
            {
                await RegisterUsers(Max_User);
                await AssignUserToGroupAsync(First_Group, Max_User);
            })
            .Verificationn(Max_User, anatoliySender)
            .Connect(olessyaSender)
            .Connect(anatoliyReceiver, nathanReceiver2, maxReceiver1);

        var graph = new InstructionGraph(anatoliySender);

        await InstructionExecuter(graph);
    }



    //public async Task GroupUsersCleanup()
    //{
    //    await GroupUsersCleanup(First_Group);
    //    await GroupUsersCleanup(Second_Group);
    //}

}
