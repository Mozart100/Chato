using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;

namespace Chato.Server.BackgroundTasks;

public class CacheEvictionBackgroundTask : BackgroundService
{
    private const int Period_Timeout = 3;
    private const int Timeout_In_Second = 10;

    //private readonly ICacheItemDelegateQueue _cacheItemDelegateQueue;
    private readonly IRoomIndexerRepository _roomIndexerRepository;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CacheEvictionBackgroundTask> _logger;

    public CacheEvictionBackgroundTask(
        IRoomIndexerRepository roomIndexerRepository,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CacheEvictionBackgroundTask> logger
        )
    {
        this._roomIndexerRepository = roomIndexerRepository;
        this._serviceScopeFactory = serviceScopeFactory;
        this._logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000 * 5);

        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                var snapshot = _roomIndexerRepository.GetAllKeyValuesSnapshot();

                foreach (var (roomName, timeStemp) in snapshot)
                {
                    if (timeStemp.IsSecondOver(Timeout_In_Second))
                    {
                        await roomService.RemoveRoomByNameOrIdAsync(roomName);
                        _logger.LogInformation($"Room {roomName} was evicted!!!.");
                    }
                    else
                    {
                        _logger.LogInformation($"Eviction Cache missed!!!.");
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(Period_Timeout), stoppingToken);
        }
    }
}