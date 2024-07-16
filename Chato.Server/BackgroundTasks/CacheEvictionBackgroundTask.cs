using Chato.Server.Configuration;
using Chato.Server.DataAccess.Repository;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;

namespace Chato.Server.BackgroundTasks;

public class CacheEvictionBackgroundTask : BackgroundService
{
    private readonly CacheEvictionRoomConfig _config;

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

                foreach (var (roomName, startUnusedTimeStamp,  ThreshholdAbsoluteEviction) in snapshot)
                {
                    if(persistentUsers.Contains(roomName) == true)
                    {
                        continue;
                    }

                    TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);
                    TimeSpan elapsed = currentTime.ToTimeSpan() - startUnusedTimeStamp.ToTimeSpan();

                    if (elapsed.TotalSeconds >= _config.UnusedTimeoutSeconds)
                    {
                        _logger.LogInformation($"UnusedTimeoutSeconds Original  for room '{roomName}': Minute = {startUnusedTimeStamp.Minute}  Scecond = {startUnusedTimeStamp.Second} and MilliSecond {startUnusedTimeStamp.Millisecond}");
                        _logger.LogInformation($"UnusedTimeoutSeconds Timestamp for room '{roomName}': Minute = {currentTime.Minute}  Scecond = {currentTime.Second} and MilliSecond {currentTime.Millisecond}");


                        var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                        await roomService.RemoveRoomByNameOrIdAsync(roomName);

                        _logger.LogInformation($"UnusedTimeoutSeconds Room {roomName} was evicted!!!.");
                    }
                    else
                    {
                        if (currentTime > ThreshholdAbsoluteEviction)
                        {
                            _logger.LogInformation($"AbsoluteEvictionInSeconds Original  for room '{roomName}': Minute = {ThreshholdAbsoluteEviction.Minute}  Scecond = {ThreshholdAbsoluteEviction.Second} and MilliSecond {ThreshholdAbsoluteEviction.Millisecond}");
                            _logger.LogInformation($"AbsoluteEvictionInSeconds Timestamp for room '{roomName}': Minute = {currentTime.Minute}  Scecond = {currentTime.Second} and MilliSecond {currentTime.Millisecond}");


                            var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                            await roomService.RemoveRoomByNameOrIdAsync(roomName);

                            _logger.LogInformation($"AbsoluteEvictionInSeconds Room {roomName} was evicted!!!.");
                        }
                        else
                        {
                            _logger.LogInformation($"Eviction Cache missed!!!.");
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(_config.PeriodTimeoutSeconds), stoppingToken);
        }
    }
}