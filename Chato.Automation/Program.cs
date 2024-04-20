using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/chat";


var fullHandShake = new FullHandShakeScenario(baseUrl);
await fullHandShake.StartRunScenario();



var groupScenario = new GroupScenario(baseUrl);
await groupScenario.StartRunScenario();


Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

