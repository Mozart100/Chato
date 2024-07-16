using Chato.Server.Configuration;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;

namespace Chato.Server.BackgroundTasks;

public class CacheEvictionBackgroundTask : BackgroundService
{
    //private const int Period_Timeout = 1;
    //private const int Timeout_In_Second = 3;
    private readonly CacheEvictionRoomConfig _config;

    //private readonly ICacheItemDelegateQueue _cacheItemDelegateQueue;
    private readonly IRoomIndexerRepository _roomIndexerRepository;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CacheEvictionBackgroundTask> _logger;

    public CacheEvictionBackgroundTask(IOptions<CacheEvictionRoomConfig> config,
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
        await Task.Delay(1000 * 3);

        var persistentUsers = new HashSet<string>(IPersistentUsers.PersistentUsers);

        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var snapshot = _roomIndexerRepository.GetAllKeyValuesSnapshot();

                foreach (var (roomName, startTime) in snapshot)
                {
                    if(persistentUsers.Contains(roomName) == true)
                    {
                        continue;
                    }

                    TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);
                    TimeSpan elapsed = currentTime.ToTimeSpan() - startTime.ToTimeSpan();


                    // Check if the specified duration has passed
                    if (elapsed.TotalSeconds >= _config.UnusedTimeoutSeconds)
                    //if (elapsed.TotalSeconds >= 5)
                    {

                        _logger.LogInformation($"Original  for room '{roomName}': Minute = {startTime.Minute}  Scecond = {startTime.Second} and MilliSecond {startTime.Millisecond}");
                        _logger.LogInformation($"Timestamp for room '{roomName}': Minute = {currentTime.Minute}  Scecond = {currentTime.Second} and MilliSecond {currentTime.Millisecond}");


                        var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
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