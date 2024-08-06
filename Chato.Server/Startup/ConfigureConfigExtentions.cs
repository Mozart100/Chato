using Chato.Server.Infrastracture;
using Microsoft.Extensions.Configuration;
using System;

namespace Chato.Server.Startup;

public static class ConfigureConfigExtentions
{
    public static void AddConfig<TConfig>(this IServiceCollection services, IConfiguration configuration)
        where TConfig : ChatoConfigBase<TConfig>
    {

        services.Configure<TConfig>(configuration.GetSection(typeof(TConfig).Name));

    }

    public static IConfigurationRoot GetConfigurationRoot(string path, string environmentName, bool addUserSecrets)
    {
        //var environmentName = environment.EnvironmentName;
        //var path = Environment.CurrentDirectory;

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


        if (environmentName.IsNullOrEmpty() == false)
        {
            builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
        }

        builder = builder.AddEnvironmentVariables();

        if (addUserSecrets)
        {
            builder.AddUserSecrets(typeof(ConfigureConfigExtentions).Assembly, true);
        }

        //new AppZureKeyValueConfigurer().Configure(builder,builtConfig);

        return builder.Build();
    }
}