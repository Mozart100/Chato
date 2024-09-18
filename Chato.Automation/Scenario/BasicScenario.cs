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
        var supervisor = $"User_{nameof(SendingWithinLobi)}";

        var activeUsers = new string[] { supervisor, Anatoliy_User, Olessya_User, Nathan_User };
        await RegisterUsers(activeUsers);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(supervisor, Anatoliy_User, Olessya_User, Nathan_User);

        users[Anatoliy_User].Step(users[Nathan_User]).Step(users[Olessya_User])
            .Step(users[Anatoliy_User].SendingToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Anatoliy_User].Logout())
            .Step(users[Olessya_User].Logout())
            .Step(users[Nathan_User].Logout())

            .Step(users[supervisor])
            .Step(users[supervisor].Do(async user =>
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
            .Step(users[supervisor].Logout())
            ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

    }

    private async Task SendingWithinLobi_UserMovedChat()
    {
        const string chat2 = "university_chat";
        var message_1 = "Shalom";
        var message_2 = "hello";

        var supervisor = $"User_{nameof(SendingWithinLobi_UserMovedChat)}";
        var activeUsers = new string[] { supervisor, Anatoliy_User, Olessya_User, Nathan_User };
        await RegisterUsers(activeUsers);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(supervisor, Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Step(users[Nathan_User]).Step(users[Olessya_User])
            .Step(users[Anatoliy_User].SendingToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Olessya_User].JoinOrCreateChat(chat2))
            .Step(users[Nathan_User].JoinOrCreateChat(chat2))

            .Step(users[Olessya_User].SendingToRestRoom(message_2, chat2, 1))
            .Step(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_2))
            .Step(users[Anatoliy_User].Is_Not_Received2222(IChatService.Lobi))

            .Step(users[Anatoliy_User].Logout())
            .Step(users[Olessya_User].Logout())
            .Step(users[Nathan_User].Logout())


            .Step(users[supervisor])
            .Step(users[supervisor].Do(async user =>
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
            .Step(users[supervisor].Logout())

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);

    }


    private async Task VerificationStep2222()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "university_chat";

        const string message_1 = $"{nameof(message_1)}";
        const string message_2 = $"{nameof(message_2)}";
        const string message_3 = $"{nameof(message_3)}";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Step(users[Nathan_User]).Step(users[Olessya_User])
            .Step(users[Anatoliy_User].SendingToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingFrom2222(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Olessya_User].JoinOrCreateChat(chat2))
            .Step(users[Nathan_User].JoinOrCreateChat(chat2))

            .Step(users[Olessya_User].SendingToRestRoom(message_2, chat2, 1))
            .Step(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_2))

            .Step(users[Anatoliy_User].JoinOrCreateChat(chat2))

            .Step(users[Olessya_User].SendingToRestRoom(message_3, chat2, 2))
            .Step(users[Nathan_User].ReceivingFrom2222(chat2, Olessya_User, message_3))
            .Step(users[Anatoliy_User].ReceivingFrom2222(chat2, Olessya_User, message_3))


            .Step(users[Anatoliy_User].Logout())
            .Step(users[Olessya_User].Logout())
            .Step(users[Nathan_User].Logout());

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);
    }
}
