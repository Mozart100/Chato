using System;
using Chato.Automation;
using Chato.Automation.Scenario;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = CreateHostBuilder(args).Build();
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



static IHostBuilder CreateHostBuilder(string[] args)
{
    string baseUrl;
    baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    // If necessary, create it.
    if (baseUrl == null)
    {
        baseUrl = "https://localhost:7138";
       
    }
    var config = new ScenarioConfig
    {
        BaseUrl = baseUrl
    };

    return Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            //logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
            //logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);
        })

        .ConfigureServices((_, services) =>
    {
        services.AddSingleton<App>();
        services.AddSingleton<ScenarioConfig>(config);
        services.AddSingleton<GroupHandShakeScenario>();
        //services.AddSingleton<HubStreamScenario>();
        services.AddSingleton<BasicScenario>();
        services.AddSingleton<RegistrationValidationScenario>();
        services.AddSingleton<RoomSendingReceivingScenario>();
        services.AddSingleton<CacheScenario>();

        





    });
}



