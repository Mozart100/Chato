using Arkovean.Chat.BackgroundTasks;
using Arkovean.Chat.DataAccess.Repository;
using Arkovean.Chat.Errors;
using Arkovean.Chat.Services;
using Arkovean.Chat.Services.Validations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;

namespace Arkovean.Chat.Startup
{
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

            //services.AddSingleton<ICityRepository, CityRepository>();
            //services.AddSingleton<ICityLocaterService, CityLocaterService>();

            services.AddSignalR();
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });


            //services.AddResponseCompression();

            services.AddSingleton<ICityLocaterValidationService, CityLocaterValidationService>();
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
}