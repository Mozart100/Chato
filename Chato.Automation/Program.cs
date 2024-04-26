using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/chat";



//var groupHandShakeScenario = new GroupHandShakeScenario(baseUrl);
//await groupHandShakeScenario.StartRunScenario();



var streamScenario = new HubStreamScenario(baseUrl);
await streamScenario.StartRunScenario();






Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

