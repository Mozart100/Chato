using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Chato.Automation;

internal class App
{
    private readonly ILogger<App> _logger;
    private readonly GroupHandShakeScenario _groupHandShakeScenario;
    //private readonly HubStreamScenario _hubStreamScenario;
    private readonly BasicScenario _basicScenario;
    private readonly RegistrationValidationScenario _registrationValidationScenario;

    public App(ILogger<App> logger, 
        GroupHandShakeScenario groupHandShakeScenario,
        //HubStreamScenario hubStreamScenario,
        BasicScenario basicScenario,
        RegistrationValidationScenario registrationValidationScenario
        )
    {
        _logger = logger;
        this._groupHandShakeScenario = groupHandShakeScenario;
        //this._hubStreamScenario = hubStreamScenario;
        this._basicScenario = basicScenario;
        this._registrationValidationScenario = registrationValidationScenario;
    }


    public async Task RunAsync(string[] args)
    {

        await _registrationValidationScenario.StartRunScenario();
        await _basicScenario.StartRunScenario();
        await _groupHandShakeScenario.StartRunScenario();
        //await _hubStreamScenario.StartRunScenario();

        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");


        Console.ReadLine();
    }
}
