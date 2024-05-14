using Chato.Server.Services;

namespace Chato.Server.BackgroundTasks;

public class PreloadBackgroundTask : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PreloadBackgroundTask(IServiceScopeFactory serviceScopeFactory)
    {
        this._serviceScopeFactory = serviceScopeFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            var preloaders = scope.ServiceProvider.GetRequiredService<IEnumerable<IPreloadDataLoader>>();

            foreach (var preloader in preloaders)
            {
                await preloader.ExecuteAsync();

            }
        }
    }
}