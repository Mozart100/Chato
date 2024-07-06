using Chato.Server.Services;

namespace Chato.Server.BackgroundTasks;

public class CacheRemovalItemBackgroundTask : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CacheRemovalItemBackgroundTask(IServiceScopeFactory serviceScopeFactory)
    {
        this._serviceScopeFactory = serviceScopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await Task.Delay(1000 * 5);
        //using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        //{
        //    var preloaders = scope.ServiceProvider.GetRequiredService<IEnumerable<IPreloadDataLoader>>();

        //    foreach (var preloader in preloaders)
        //    {
        //        await preloader.ExecuteAsync();

        //    }
        //}
    }
}
