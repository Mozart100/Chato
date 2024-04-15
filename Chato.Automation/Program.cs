using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/chat";


var twoUsersHandShake = new TwoUsersHandShakeScenario(baseUrl);
await twoUsersHandShake.StartRunScenario();



//var scenario = new ThreeUsersHandShakeScenario(baseUrl);
//await scenario.StartRunScenario();


Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

