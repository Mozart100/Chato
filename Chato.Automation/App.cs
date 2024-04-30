using Arkovean.Chat.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Chato.Automation;

internal class App
{
    private readonly ILogger<App> _logger;
    private readonly GroupHandShakeScenario _groupHandShakeScenario;
    private readonly HubStreamScenario _hubStreamScenario;

    public App(ILogger<App> logger, 
        GroupHandShakeScenario groupHandShakeScenario,
        HubStreamScenario hubStreamScenario
        )
    {
        _logger = logger;
        this._groupHandShakeScenario = groupHandShakeScenario;
        this._hubStreamScenario = hubStreamScenario;
    }


    public async Task RunAsync(string[] args)
    {
       
        await _groupHandShakeScenario.StartRunScenario();
        await _hubStreamScenario.StartRunScenario();


        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");


        Console.ReadLine();
    }
}
