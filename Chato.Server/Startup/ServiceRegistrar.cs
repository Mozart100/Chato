using Chato.Server.BackgroundTasks;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Errors;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;
using Chato.Server.Services.Validations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Chato.Server.Startup;

public static class ServiceRegistrar
{
    public const string CorsPolicy = "CorsPolicy";
    public static IServiceCollection CustomServiceRegistration(this IServiceCollection services, ConfigurationManager  configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, builder => builder.WithOrigins("http://localhost:4200").
                                                               AllowAnyMethod().
                                                               AllowAnyHeader().
                                                               AllowCredentials());
        });

        services.AddSingleton<IRoomRepository, RoomRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IRoomIndexerRepository, RoomIndexerRepository>();
        //services.AddSingleton<IPreloadDataLoader, GenerateDefaultRoomAndUsersService>();




        //services.Decorate<IUserRepository, DelegateQueueUserRepository>();


        services.AddSingleton<ILockerDelegateQueue, LockerDelegateQueue>();
        services.AddSingleton<ICacheItemDelegateQueue, CacheItemDelegateQueue>();


        services.AddSignalR();
        services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });

        //services.AddTransient<ProblemDetailsFactory, ProblemDetailsAdvanceFeaturesFactory>();

        services.AddHostedService<PreloadBackgroundTask>();
        services.AddHostedService<DelegateQueueBackgroundTask>();
        services.AddHostedService<CacheEvictionBackgroundTask>();





        //kcservices.AddMiddleware


        return services;
    }

    public static IServiceCollection NativeServiceRegistration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddAutoMapper(typeof(Program).Assembly);

        services.AddMemoryCache();


       services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRegistrationValidationService, RegistrationValidationService>();


        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();



        services.AddHttpContextAccessor();


        return services;
    }
}