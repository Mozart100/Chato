using Chato.Automation.Scenario;
using Microsoft.Extensions.Logging;

namespace Chato.Automation;

internal class TestPlan
{
    private readonly ILogger<TestPlan> _logger;
    private readonly BasicScenario _basicScenario;
    private readonly RegistrationValidationScenario _registrationValidationScenario;
    private readonly RoomSendingReceivingScenario roomSendingReceivingScenario;
    //private readonly CacheScenario _cacheScenario;

    public TestPlan(ILogger<TestPlan> logger,
        BasicScenario basicScenario,
        RegistrationValidationScenario registrationValidationScenario,
        RoomSendingReceivingScenario roomSendingReceivingScenario
        //CacheScenario cacheScenario
        )
    {
        _logger = logger;
        this._basicScenario = basicScenario;
        this._registrationValidationScenario = registrationValidationScenario;
        this.roomSendingReceivingScenario = roomSendingReceivingScenario;
        //this._cacheScenario = cacheScenario;
    }


    public async Task RunAsync(string[] args)
    {
        try
        {
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"{i+1} Check started!!!!!");
                Console.WriteLine($"{i+1} Check started!!!!!");
                Console.WriteLine($"{i+1} Check started!!!!!");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine($"-------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                await _registrationValidationScenario.StartRunScenario();
                await _basicScenario.StartRunScenario();
                await roomSendingReceivingScenario.StartRunScenario();


            }

        }
        catch (Exception ex)
        {
            Console.ReadLine();
            throw;
        }


        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");
        Console.WriteLine("All test passed successfully!!!!!");


        Console.ReadLine();
    }
}
