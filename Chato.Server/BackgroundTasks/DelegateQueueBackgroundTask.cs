using Chato.Server.Infrastracture.QueueDelegates;

namespace Chato.Server.BackgroundTasks;

public class DelegateQueueBackgroundTask : BackgroundService
{
    private readonly ILockerDelegateQueue _delegateQueue;

    public DelegateQueueBackgroundTask( ILockerDelegateQueue delegateQueue)
    {
        this._delegateQueue = delegateQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Func<Task>  callback = await _delegateQueue.PopOrWaitAsync(stoppingToken);

            if(callback is not null)
            {
                await callback();
            }
        }
    }
}