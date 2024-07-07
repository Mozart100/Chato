using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;

namespace Chato.Server.BackgroundTasks;

public class CacheRemovalItemBackgroundTask : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICacheItemDelegateQueue _cacheDelegateQueue;

    public CacheRemovalItemBackgroundTask(IServiceScopeFactory serviceScopeFactory,
        ICacheItemDelegateQueue cacheRemovableIteDelegateQueue
        )
    {
        this._serviceScopeFactory = serviceScopeFactory;
        this._cacheDelegateQueue = cacheRemovableIteDelegateQueue;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    var result = await _cacheDelegateQueue.PopOrWaitAsync(stoppingToken);

        //    if (result is not null)
        //    {
        //        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        //        {
        //            var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
        //            await roomService.RemoveRoomByNameOrIdAsync(result.RoomNameOrId);
        //        }
        //    }
        //}
    }
}
