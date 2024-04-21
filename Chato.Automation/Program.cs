using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/chat";


var fullHandShake = new FullHandShakeScenario(baseUrl);
await fullHandShake.StartRunScenario();



var groupHandShakeScenario = new GroupHandShakeScenario(baseUrl);
await groupHandShakeScenario.StartRunScenario();


Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

