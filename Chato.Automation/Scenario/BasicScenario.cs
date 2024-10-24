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


    //private const string Natali_User = "natali";
    //private const string Max_User = "max";
    //private const string Idan_User = "itan";

    public BasicScenario(ILogger<BasicScenario> logger, ScenarioConfig config) : base(logger, config)
    {

        BusinessLogicCallbacks.Add(SendingImagesOnlyBetweenTwoPeople);
        BusinessLogicCallbacks.Add(SendingWithinLobi);

        BusinessLogicCallbacks.Add(SendingOnlyBetweenTwoPeople);
        BusinessLogicCallbacks.Add(SendingWithinLobi_UserMovedChat);
        BusinessLogicCallbacks.Add(VerificationStep);
        BusinessLogicCallbacks.Add(GetHistoryWithoutJoin);



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
            .Step(users[Anatoliy_User].SendingTextToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))

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

    private async Task SendingOnlyBetweenTwoPeople()
    {
        var message_1 = "Shalom";
        var message_2 = $"message 2";

        var chat2 = IChatService.GetChatName(Anatoliy_User, Nathan_User);

        var activeUsers = new string[] { Anatoliy_User, Nathan_User };
        await RegisterUsers(activeUsers);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Nathan_User);

        users[Anatoliy_User].Step(users[Nathan_User])

            .Step(users[Anatoliy_User].SendingTextToRestRoom(message_1, chat2, 1))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Anatoliy_User, message_1))

            .Step(users[Nathan_User].SendingTextToRestRoom(message_2, chat2, 1))
            .Step(users[Anatoliy_User].ReceivingMessage(chat2, Nathan_User, message_2))

            .Step(users[Anatoliy_User].Logout())
            .Step(users[Nathan_User].Logout())

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
            .Step(users[Anatoliy_User].SendingTextToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Olessya_User].JoinOrCreateChat(chat2, 1))
            .Step(users[Nathan_User].JoinOrCreateChat(chat2, 2))

            .Step(users[Olessya_User].NotifyUser(chat2))
            .Step(users[Olessya_User].NotifyUser(chat2))
            .Step(users[Nathan_User].NotifyUser(chat2))



            .Step(users[Olessya_User].SendingTextToRestRoom(message_2, chat2, 1))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Olessya_User, message_2))
            .Step(users[Anatoliy_User].Is_Not_ReceivedMessage(IChatService.Lobi))

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


    private async Task VerificationStep()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "university_chat";

        const string message_1 = $"{nameof(message_1)}";
        const string message_2 = $"{nameof(message_2)}";
        const string message_3 = $"{nameof(message_3)}";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Step(users[Nathan_User]).Step(users[Olessya_User])
            .Step(users[Anatoliy_User].SendingTextToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Olessya_User].JoinOrCreateChat(chat2))
            .Step(users[Nathan_User].JoinOrCreateChat(chat2))

            .Step(users[Olessya_User].SendingTextToRestRoom(message_2, chat2, 1))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Olessya_User, message_2))

            .Step(users[Anatoliy_User].JoinOrCreateChat(chat2, 4))
            .Step(users[Olessya_User].NotifyUser(chat2))
            .Step(users[Nathan_User].NotifyUser(chat2))
            .Step(users[Anatoliy_User].NotifyUser(chat2))

            .Step(users[Olessya_User].SendingTextToRestRoom(message_3, chat2, 2))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Olessya_User, message_3))
            .Step(users[Anatoliy_User].ReceivingMessage(chat2, Olessya_User, message_3))


            .Step(users[Anatoliy_User].Logout())
            .Step(users[Olessya_User].Logout())
            .Step(users[Nathan_User].Logout());

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);

        await InstructionExecuter(graph);
    }

    private async Task GetHistoryWithoutJoin()
    {
        await RegisterUsers(Anatoliy_User, Olessya_User, Nathan_User);

        const string chat2 = "second_chat";

        const string message_1 = $"{nameof(message_1)}";
        const string message_2 = $"{nameof(message_2)}";
        const string message_3 = $"{nameof(message_3)}";

        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Olessya_User, Nathan_User);


        users[Anatoliy_User].Step(users[Nathan_User]).Step(users[Olessya_User])
            .Step(users[Anatoliy_User].SendingTextToRestRoom(message_1, IChatService.Lobi, 2))
            .Step(users[Nathan_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))
            .Step(users[Olessya_User].ReceivingMessage(IChatService.Lobi, Anatoliy_User, message_1))

            .Step(users[Olessya_User].JoinOrCreateChat(chat2))
            .Step(users[Nathan_User].JoinOrCreateChat(chat2))

            .Step(users[Olessya_User].SendingTextToRestRoom(message_2, chat2, 1))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Olessya_User, message_2))


            .Step(users[Nathan_User].SendingTextToRestRoom(message_3, chat2, 1))
            .Step(users[Olessya_User].ReceivingMessage(chat2, Nathan_User, message_3))

            .Step(users[Anatoliy_User].GetHistoryChat(chat2, 4))

            .Step(users[Anatoliy_User].Logout())
            .Step(users[Olessya_User].Logout())
            .Step(users[Nathan_User].Logout());

        ;


        var graph = new InstructionGraph(users[Anatoliy_User]);
        await InstructionExecuter(graph);
    }

    private async Task SendingImagesOnlyBetweenTwoPeople()
    {
        var imagepath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "test.jpeg");
        var expectedFilePath = $"{IChatService.ChatImages}/anatoliy__nathan/3.jpeg";

        var message_1 = "Shalom";
        var message_image_2 = ConvertFileToBase64(imagepath);

        var chat2 = IChatService.GetChatName(Anatoliy_User, Nathan_User);

        var activeUsers = new string[] { Anatoliy_User, Nathan_User };
        await RegisterUsers(activeUsers);
        var users = InstructionNodeFluentApi.RegisterInLoLobi(Anatoliy_User, Nathan_User);

        users[Anatoliy_User].Step(users[Nathan_User])

            .Step(users[Anatoliy_User].SendingTextToRestRoom(message_1, chat2, 1))
            .Step(users[Nathan_User].ReceivingMessage(chat2, Anatoliy_User, message_1))

            //.Step(users[Nathan_User].SendingTextToRestRoom(message_image_2, chat2, 1,SenderInfoType.TextMessage))
            .Step(users[Nathan_User].SendingTextToRestRoom(message_image_2, chat2, 1, SenderInfoType.Image, expectedFilePath))
            .Step(users[Anatoliy_User].ReceivingMessage(chat2, Nathan_User, null, SenderInfoType.Image, expectedFilePath))

            .Step(users[Anatoliy_User].Logout())
            .Step(users[Nathan_User].Logout())

;

        var graph = new InstructionGraph(users[Anatoliy_User]);
        await InstructionExecuter(graph);

        var ptr = await GetImage(ImagePathCOmbine(expectedFilePath));
        var downloadFile = Convert.ToBase64String(ptr);
        downloadFile.Should().Be(message_image_2);
    }

    public static string ConvertFileToBase64(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);

        return base64String;
    }
}
