using Chato.Server.Infrastracture;

namespace Chato.Server.BackgroundTasks;

public class DelegateQueueBackgroundTask : BackgroundService
{
    private readonly IDelegateQueue delegateQueue;

    public DelegateQueueBackgroundTask(IDelegateQueue delegateQueue)
    {
        this.delegateQueue = delegateQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        delegateQueue.Initialize(stoppingToken);
    }
}
