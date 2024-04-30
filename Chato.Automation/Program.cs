using Arkovean.Chat.Automation.Scenario;
using Chato.Automation;
using Chato.Automation.Scenario;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = CReateHostBuilder(args).Build();
using var scop = host.Services.CreateScope();
var services = scop.ServiceProvider;

try
{
    var app = services.GetRequiredService<App>();
    await app.RunAsync(null);

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}



static IHostBuilder CReateHostBuilder(string[] args)
{
    var config = new ScenarioConfig
    {
        BaseUrl = "https://localhost:7138/chat"
    };

    return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
    {
        services.AddSingleton<App>();
        services.AddSingleton<ScenarioConfig>(config);
        services.AddSingleton<GroupHandShakeScenario>();
        services.AddSingleton<HubStreamScenario>();


    });
}



