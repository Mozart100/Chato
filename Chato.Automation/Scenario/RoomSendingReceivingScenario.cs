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
        BusinessLogicCallbacks.Add(ClosingChat);
        //BusinessLogicCallbacks.Add(SendingInsideTheRoom);
        //BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


        //BusinessLogicCallbacks.Add(Setup_SendingInsideTheRoom_Step);
        //BusinessLogicCallbacks.Add(RoomIsRemovedAfterEveryoneLeft);
        //BusinessLogicCallbacks.Add(async () => await UsersCleanup(Users.Keys.ToArray()));


    }

    public override string ScenarioName => "Room behaviour";
    public override string Description => "Sending and receiving messages in the room";

    private async Task ClosingChat()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "beach_chat";


        var message_1 = "Shalom";
        var message_2 = "hello";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);
        var url = string.Format(SpecificRoomTemplatesUrl, chat2);

        users[Anatoliy_User].Step(users[Nathan_User]).Step(users[Olessya_User])
            .Step(users[Anatoliy_User].SendingToRestRoom(message_1, IChatService.Lobi,2))
            .Step(users[Nathan_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Olessya_User].JoinOrCreateChat(chat2))
            .Step(users[Nathan_User].JoinOrCreateChat(chat2))

            .Step(users[Olessya_User].SendingToRestRoom(message_2, chat2,1))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Olessya_User, message_2))
            .Step(users[Anatoliy_User].Do(async user =>
            {
                var token = user.RegistrationResponse.Token;
                var response = await Get<ResponseWrapper<GetRoomResponse>>(url, token);
                response.Body.Chat.Should().NotBeNull();
            }))


            .Step(users[Olessya_User].LeaveRoom(chat2))
            .Step(users[Nathan_User].LeaveRoom(chat2))
            .Step(users[Anatoliy_User].Do(async user =>
            {
                var token = user.RegistrationResponse.Token;
                var response = await Get<ResponseWrapper<GetRoomResponse>>(url, token);
                response.Body.Chat.Should().BeNull();
            }))

            .Step(users[Anatoliy_User].Logout())
            .Step(users[Olessya_User].Logout())
            .Step(users[Nathan_User].Logout());

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

    }


    //private async Task Setup_SendingInsideTheRoom_Step()
    //{
    //    await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
    //    await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    //}
}
