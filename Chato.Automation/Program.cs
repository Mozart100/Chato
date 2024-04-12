using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/chat";



//var twoUsersListenScenario = new TwoUsersListenChatScenario(baseUrl);
//await twoUsersListenScenario.StartRunScenario();



var twoUserSendingScenario = new TwoUsersSendMessageScenario(baseUrl);
await twoUserSendingScenario.StartRunScenario();


Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

