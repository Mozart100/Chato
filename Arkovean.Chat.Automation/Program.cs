using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/chat";


//var staticDataScenario = new CitiesStaticDataScenario(baseUrl);
//await staticDataScenario.StartRunScenario();

//Console.WriteLine();
//Console.WriteLine();



//var cityLocatorScenario = new CityLocatorScenario(baseUrl);
//await cityLocatorScenario.StartRunScenario();

//Console.WriteLine();
//Console.WriteLine();

//await Task.Delay(1000 * 10);

var pingScenario = new PingChatScenario(baseUrl);
await pingScenario.StartRunScenario();


Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

