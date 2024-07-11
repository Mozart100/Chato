using Chato.Server.Configuration;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;
using Microsoft.Extensions.Options;

namespace Chato.Server.BackgroundTasks;

public class CacheEvictionBackgroundTask : BackgroundService
{
    //private const int Period_Timeout = 1;
    //private const int Timeout_In_Second = 3;
    private readonly EvictionRoomConfig _config;

    //private readonly ICacheItemDelegateQueue _cacheItemDelegateQueue;
    private readonly IRoomIndexerRepository _roomIndexerRepository;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CacheEvictionBackgroundTask> _logger;

    public CacheEvictionBackgroundTask(IOptions<EvictionRoomConfig> config,
        IRoomIndexerRepository roomIndexerRepository,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CacheEvictionBackgroundTask> logger
        )
    {

        _config = config.Value;
        this._roomIndexerRepository = roomIndexerRepository;
        this._serviceScopeFactory = serviceScopeFactory;
        this._logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000 * 5);
                   
        var persistentUsers = new HashSet<string>(IPersistentUsers.PersistentUsers);

        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                var snapshot = _roomIndexerRepository.GetAllKeyValuesSnapshot();

                foreach (var (roomName, timeStemp) in snapshot)
                {
                    if(persistentUsers.Contains(roomName) == true)
                    {
                        continue;
                    }

                    if (timeStemp.IsSecondOver(_config.UnusedTimeoutSeconds))
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

            await Task.Delay(TimeSpan.FromSeconds(_config.PeriodTimeoutSeconds), stoppingToken);
        }
    }
}