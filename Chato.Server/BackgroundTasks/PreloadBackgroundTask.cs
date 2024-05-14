using Chato.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chato.Server.BackgroundTasks
{
    public class PreloadBackgroundTask : BackgroundService
    {
        private readonly IPreloadDataLoader _preloadDataLoader;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public PreloadBackgroundTask(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                var preloader=
                    scope.ServiceProvider.GetRequiredService<IPreloadDataLoader>();


                await preloader.ExecuteAsync();
            }
        }
    }
}