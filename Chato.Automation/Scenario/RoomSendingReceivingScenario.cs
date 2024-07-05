using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.DataAccess.Models;
using Chatto.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class RoomSendingReceivingScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";


    public RoomSendingReceivingScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {
        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(SendingInsideTheRoom);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


        BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        BusinessLogicCallbacks.Add(RoomIsRemovedAfterEveryoneLeft);
        BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


    }

    public override string ScenarioName => "Room behaviour";
    public override string Description => "Sending and receiving messages in the room";

    private async Task SendingInsideTheRoom()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
        var olessyaSender = firstGroup.ReceivingFrom(Olessya_User, anatoliySender.UserName);
        var nathanReceiver = firstGroup.ReceivingFrom(Nathan_User, anatoliySender.UserName);

        var maxReceiver = firstGroup.Is_Not_Received(Max_User);



        anatoliySender.Connect(olessyaSender, nathanReceiver, maxReceiver);

        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);
    }


    private async Task RoomIsRemovedAfterEveryoneLeft()
    {
        var message_1 = "Shalom";
        var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


        var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
        var olessyaSender = firstGroup.ReceivingFrom(Olessya_User, anatoliySender.UserName);
        var nathanReceiver = firstGroup.ReceivingFrom(Nathan_User, anatoliySender.UserName);

        var maxReceiver = firstGroup.Is_Not_Received(Max_User);


        var url = string.Format(SpecificRoomTemplatesUrl, First_Group);
        var response = default(ChatRoomDto);
        anatoliySender.Connect(olessyaSender, nathanReceiver, maxReceiver)
            .Do(maxReceiver, async user=> {

                response = await Get<ChatRoomDto>(url);
                response.Should().NotBeNull();
            }).LeaveRoom( Anatoliy_User,Olessya_User,Nathan_User) ;


        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);


        response = await Get<ChatRoomDto>(url);
        response.Should().BeNull();
    }

    private async Task Setup_SendingInsideTheRoom_Step()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }
}
