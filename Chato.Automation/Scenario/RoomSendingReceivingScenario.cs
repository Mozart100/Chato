using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
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
        BusinessLogicCallbacks.Add(SendingWithinLobi_UserMovedChat);
        //BusinessLogicCallbacks.Add(SendingInsideTheRoom);
        //BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


        //BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        //BusinessLogicCallbacks.Add(RoomIsRemovedAfterEveryoneLeft);
        //BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


    }

    public override string ScenarioName => "Room behaviour";
    public override string Description => "Sending and receiving messages in the room";

    private async Task SendingWithinLobi_UserMovedChat()
    {
        await RegisterUsers2222(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "beach_chat";


        var message_1 = "Shalom";
        var message_2 = "hello";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);
        var url = string.Format(SpecificRoomTemplatesUrl, chat2);

        users[Anatoliy_User].Connect(users[Nathan_User]).Connect(users[Olessya_User])
            .Connect(users[Anatoliy_User].SendingToRestRoom222(message_1, IChatService.Lobi))
            .Connect(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Connect(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Connect(users[Olessya_User].JoinOrCreateChat(chat2))
            .Connect(users[Nathan_User].JoinOrCreateChat(chat2))

            .Connect(users[Olessya_User].SendingToRestRoom222(message_2, chat2))
            .Connect(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_2))
            .Connect(users[Anatoliy_User].Do2222(async user =>
            {
                var token = user.RegistrationResponse.Token;
                var response = await Get<ResponseWrapper<GetRoomResponse>>(url, token);
                response.Body.Chat.Should().NotBeNull();
            }))


            .Connect(users[Olessya_User].LeaveRoom222(chat2))
            .Connect(users[Nathan_User].LeaveRoom222(chat2))
            .Connect(users[Anatoliy_User].Do2222(async user =>
            {
                var token = user.RegistrationResponse.Token;
                var response = await Get<ResponseWrapper<GetRoomResponse>>(url, token);
                response.Body.Chat.Should().BeNull();
            }))

            .Connect(users[Anatoliy_User].Logout())
            .Connect(users[Olessya_User].Logout())
            .Connect(users[Nathan_User].Logout());

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

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
        var response = default(ResponseWrapper<GetRoomResponse>);

        string token = string.Empty;

        anatoliySender.Connect(olessyaSender, nathanReceiver, maxReceiver)
            .Do(maxReceiver, async user =>
            {

                token = user.RegistrationResponse.Token;
                response = await Get<ResponseWrapper<GetRoomResponse>>(url, token);
                response.Body.Chat.Should().NotBeNull();
            }).LeaveRoom(Anatoliy_User, Olessya_User, Nathan_User);


        var graph = new InstructionGraph(anatoliySender);
        await InstructionExecuter(graph);

        response = await Get<ResponseWrapper<GetRoomResponse>>(url, token);
        response.Body.Chat.Should().BeNull();
    }

    private async Task Setup_SendingInsideTheRoom_Step()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
        await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    }
}
