using Chato.Server.Infrastracture;

namespace Chato.Server.BackgroundTasks;

public class StarterDelegateQueueBackgroundTask : BackgroundService
{
    private readonly IDelegateQueue delegateQueue;

    public StarterDelegateQueueBackgroundTask(IDelegateQueue delegateQueue)
    {
        this.delegateQueue = delegateQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        delegateQueue.Initialize(stoppingToken);
    }
}
