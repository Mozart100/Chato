using Chato.Server.Infrastracture;

namespace Chato.Server.Startup;

public static class ConfigureConfigExtentions
{
    public static void AddConfig<TConfig>(this IServiceCollection services,IConfiguration configuration)
        where TConfig :ChatoConfigBase<TConfig>
    {
        
        services.Configure<TConfig>(configuration.GetSection(typeof(TConfig).Name));

    }
}