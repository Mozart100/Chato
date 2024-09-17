using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using Chato.Server.Services;
using Chatto.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Chato.Automation.Scenario;

internal class BasicScenario : InstructionScenarioBase
{
    private const string First_Group = "haifa";

    private const string Anatoliy_User = "anatoliy";
    private const string Olessya_User = "olessya";
    private const string Nathan_User = "nathan";


    private const string Natali_User = "natali";
    private const string Max_User = "max";
    private const string Idan_User = "itan";

    public BasicScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        BusinessLogicCallbacks.Add(SendingWithinLobi);
        BusinessLogicCallbacks.Add(SendingWithinLobi_UserMovedChat);
        BusinessLogicCallbacks.Add(VerificationStep2222);

        //SummaryLogicCallback.Add(CheckAllCleaned);

    }


    public override string ScenarioName => "All sort of mini scenarios";

    public override string Description => "To run mini scenarios.";

    private async Task SendingWithinLobi()
    {
        var message_1 = "Shalom";
        var supervisor = nameof(SendingWithinLobi);

        var activeUsers = new string[] { supervisor, Anatoliy_User, Olessya_User, Nathan_User };
        await RegisterUsers2222(activeUsers);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(supervisor, Anatoliy_User, Olessya_User, Nathan_User);

        users[Anatoliy_User].Connect(users[Nathan_User]).Connect(users[Olessya_User])
            .Connect(users[Anatoliy_User].SendingToRestRoom222(message_1, IChatService.Lobi, 2))
            .Connect(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Connect(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Connect(users[Anatoliy_User].Logout())
            .Connect(users[Olessya_User].Logout())
            .Connect(users[Nathan_User].Logout())

            .Connect(users[supervisor])
            .Connect(users[supervisor].Do2222(async user =>
                  {
                      var token = user.RegistrationResponse.Token;
                      var response = await Get<ResponseWrapper<GetAllUserResponse>>(GetAllUsersUrl, token);
                      foreach (var activeUser in activeUsers.Skip(1))
                      {
                          foreach (var item in response.Body.Users)
                          {
                              if (item.Equals(activeUser))
                              {
                                  throw new Exception($"This user should have been logout!!!");
                              }
                          }
                      }
                  }))
            .Connect(users[supervisor].Logout())
            ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

    }

    private async Task SendingWithinLobi_UserMovedChat()
    {
        const string chat2 = "university_chat";
        var message_1 = "Shalom";
        var message_2 = "hello";

        var supervisor = nameof(SendingWithinLobi_UserMovedChat);
        var activeUsers = new string[] { supervisor, Anatoliy_User, Olessya_User, Nathan_User };
        await RegisterUsers2222(activeUsers);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(supervisor, Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Connect(users[Nathan_User]).Connect(users[Olessya_User])
            .Connect(users[Anatoliy_User].SendingToRestRoom222(message_1, IChatService.Lobi, 2))
            .Connect(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Connect(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Connect(users[Olessya_User].JoinOrCreateChat(chat2))
            .Connect(users[Nathan_User].JoinOrCreateChat(chat2))

            .Connect(users[Olessya_User].SendingToRestRoom222(message_2, chat2, 1))
            .Connect(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_2))
            .Connect(users[Anatoliy_User].Is_Not_Received2222(IChatService.Lobi))

            .Connect(users[Anatoliy_User].Logout())
            .Connect(users[Olessya_User].Logout())
            .Connect(users[Nathan_User].Logout())


            .Connect(users[supervisor])
            .Connect(users[supervisor].Do2222(async user =>
              {
                  var token = user.RegistrationResponse.Token;
                  var response = await Get<ResponseWrapper<GetAllUserResponse>>(GetAllUsersUrl, token);
                  foreach (var activeUser in activeUsers.Skip(1))
                  {
                      foreach (var item in response.Body.Users)
                      {
                          if (item.Equals(activeUser))
                          {
                              throw new Exception($"This user should have been logout!!!");
                          }
                      }
                  }
              }))
            .Connect(users[supervisor].Logout())

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

    }


    private async Task VerificationStep2222()
    {
        await RegisterUsers2222(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "university_chat";

        const string message_1 = $"{nameof(message_1)}";
        const string message_2 = $"{nameof(message_2)}";
        const string message_3 = $"{nameof(message_3)}";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Connect(users[Nathan_User]).Connect(users[Olessya_User])
            .Connect(users[Anatoliy_User].SendingToRestRoom222(message_1, IChatService.Lobi, 2))
            .Connect(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Connect(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Connect(users[Olessya_User].JoinOrCreateChat(chat2))
            .Connect(users[Nathan_User].JoinOrCreateChat(chat2))

            .Connect(users[Olessya_User].SendingToRestRoom222(message_2, chat2, 1))
            .Connect(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_2))

            .Connect(users[Anatoliy_User].JoinOrCreateChat(chat2))

            .Connect(users[Olessya_User].SendingToRestRoom222(message_3, chat2, 2))
            .Connect(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_3))
            .Connect(users[Anatoliy_User].ReceivingFrom2222(chat2, Olessya_User, message_3))


            .Connect(users[Anatoliy_User].Logout())
            .Connect(users[Olessya_User].Logout())
            .Connect(users[Nathan_User].Logout());

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);
    }



    //private async Task VerificationStep()
    //{
    //    var message_1 = "Shalom";
    //    var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


    //    var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
    //    var olessyaSender = firstGroup.SendingToRestRoom(Olessya_User);
    //    var maxReceiver1 = firstGroup.ReceivingFrom(Max_User, olessyaSender.UserName);

    //    anatoliySender.Connect(olessyaSender)
    //        .Do(maxReceiver1, async user =>
    //        {
    //            await RegisterUsers(Max_User);
    //            await AssignUserToGroupAsync(First_Group, Max_User);
    //        })
    //        .ReceivedVerification(Max_User, anatoliySender, olessyaSender);

    //    var graph = new InstructionGraph(anatoliySender);
    //    await InstructionExecuter(graph);
    //}


    //private async Task SendingOnlyToRoomStep()
    //{
    //    var message_1 = "Shalom";
    //    var firstGroup = InstructionNodeFluentApi.StartWithGroup(groupName: First_Group, message_1);


    //    var anatoliySender = firstGroup.SendingToRestRoom(Anatoliy_User);
    //    var olessyaSender = firstGroup.ReceivingFrom(Olessya_User, anatoliySender.UserName);
    //    var nathanReceiver = firstGroup.ReceivingFrom(Nathan_User, anatoliySender.UserName);

    //    var maxReceiver = firstGroup.Is_Not_Received(Max_User);



    //    anatoliySender.Connect(olessyaSender, nathanReceiver, maxReceiver);

    //    var graph = new InstructionGraph(anatoliySender);
    //    await InstructionExecuter(graph);
    //}


    //private async Task SetupGroup()
    //{
    //    await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User);
    //    await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    //}

    //private async Task Setup_SendingOnlyToRoomStep_Step()
    //{
    //    await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User, Max_User);
    //    await AssignUserToGroupAsync(First_Group, Anatoliy_User, Olessya_User, Nathan_User);
    //}

}
