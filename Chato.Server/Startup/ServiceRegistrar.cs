using Chato.Server.BackgroundTasks;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Errors;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;

namespace Chato.Server.Startup;

public static class ServiceRegistrar
{
    public const string CorsPolicy = "CorsPolicy";
    public static IServiceCollection CustomServiceRegistration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, builder => builder.WithOrigins("http://localhost:4200").
                                                               AllowAnyMethod().
                                                               AllowAnyHeader().
                                                               AllowCredentials());
        });

        services.AddSingleton<IChatRoomRepository, ChatRoomRepository>(); 

        services.AddSignalR();
        services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });


        //services.AddResponseCompression();

        services.AddSingleton<ProblemDetailsFactory, ProblemDetailsAdvanceFeaturesFactory>();



        services.AddHostedService<TestBackgroundTask>();

        return services;
    }

    public static IServiceCollection NativeServiceRegistration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAutoMapper(typeof(Program).Assembly);

        return services;
    }
}