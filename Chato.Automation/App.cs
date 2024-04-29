using Arkovean.Chat.Automation.Scenario;
using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Chato.Automation;

internal class App
{
    private readonly ILogger<App> _logger;

    public App(ILogger<App> logger)
    {
        _logger = logger;
    }


    public async Task RunAsync(string[] args)
    {
        var config = new ScenarioConfig
        {
            BaseUrl = "https://localhost:7138/chat"
        };


        var groupHandShakeScenario = new GroupHandShakeScenario(_logger, config);
        await groupHandShakeScenario.StartRunScenario();



        var streamScenario = new HubStreamScenario(_logger, config);
        await streamScenario.StartRunScenario();


        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");


        Console.ReadLine();
    }
}
