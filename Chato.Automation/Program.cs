using System;
using Chato.Automation;
using Chato.Automation.Scenario;
using Chato.Server.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = CreateHostBuilder(args).Build();
using var scop = host.Services.CreateScope();
var services = scop.ServiceProvider;

try
{
    var app = services.GetRequiredService<TestPlan>();
    await app.RunAsync(null);

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}



static IHostBuilder CreateHostBuilder(string[] args)
{
    string baseUrl = null;

    //How to know in what configuration build I run this project ?????

    //if (baseUrl == null)
    //{
    //    var localEnvironment = ConfigurationLoader.GetConfigurationRoot(Directory.GetCurrentDirectory(), "Aws");
    //    var testPlanSection = localEnvironment.GetSection(TestPlanConfig.ApiName);
    //    var testPlanConfig = testPlanSection.Get<TestPlanConfig>();


    //    baseUrl = testPlanConfig.BaseUrl;
    //    //baseUrl = "https://localhost:7138";

    
    //var config = new ScenarioConfig
    //{
    //    BaseUrl = baseUrl
    //};

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            //logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
            //logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);
        })
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            //var env = hostingContext.HostingEnvironment;

            //Console.WriteLine($"Current build configuration: {env.EnvironmentName}");

            //if (env.IsDevelopment())
            //{
            //}
            //else if (env.EnvironmentName == "Aws")
            //{
            //}
        })

        .ConfigureServices((_, services) =>
        {


            var localEnvironment = ConfigurationLoader.GetConfigurationRoot(Directory.GetCurrentDirectory()); 
            //var localEnvironment = ConfigurationLoader.GetConfigurationRoot(Directory.GetCurrentDirectory(), "Aws");
            var testPlanSection = localEnvironment.GetSection(TestPlanConfig.ApiName);
            var testPlanConfig = testPlanSection.Get<TestPlanConfig>();


            baseUrl = testPlanConfig.BaseUrl;
            //baseUrl = "https://localhost:7138";


            var config = new ScenarioConfig
            {
                BaseUrl = baseUrl
            };



            services.AddSingleton<TestPlan>();
            services.AddSingleton<ScenarioConfig>(config);
            services.AddSingleton<GroupHandShakeScenario>();
            //services.AddSingleton<HubStreamScenario>();
            services.AddSingleton<BasicScenario>();
            services.AddSingleton<RegistrationValidationScenario>();
            services.AddSingleton<RoomSendingReceivingScenario>();
            services.AddSingleton<CacheScenario>();







        });


    return host;
}



