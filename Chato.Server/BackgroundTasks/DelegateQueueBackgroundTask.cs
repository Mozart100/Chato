using Chato.Server.Infrastracture.QueueDelegates;

namespace Chato.Server.BackgroundTasks;

public class DelegateQueueBackgroundTask : BackgroundService
{
    private readonly ILogger<DelegateQueueBackgroundTask> logger;
    private readonly ILockerDelegateQueue _delegateQueue;

    public DelegateQueueBackgroundTask( ILogger<DelegateQueueBackgroundTask> logger ,   ILockerDelegateQueue delegateQueue)
    {
        this.logger = logger;
        this._delegateQueue = delegateQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Func<Task> callback = await _delegateQueue.PopOrWaitAsync(stoppingToken);

            if (callback is not null)
            {
                try
                {
                    await callback();
                }
                catch (Exception ex)
                {
                    var target = callback.Target;
                    this.logger.LogError(ex.ToString());
                }
            }

        }
    }
}