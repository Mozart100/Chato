using Arkovean.Chat.Automation.Scenario;

var baseUrl = "https://localhost:7138/cities";


var staticDataScenario = new CitiesStaticDataScenario(baseUrl);
await staticDataScenario.StartRunScenario();

Console.WriteLine();
Console.WriteLine();



var cityLocatorScenario = new CityLocatorScenario(baseUrl);
await cityLocatorScenario.StartRunScenario();

Console.WriteLine();
Console.WriteLine();


Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");


Console.ReadLine();

