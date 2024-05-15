namespace Chato.Server.Infrastracture;


public class DelegateQueue
{
    private readonly Queue<Func<Task>> _delegates;
    private readonly CancellationToken _cancellationToken;
    private readonly Task backgroundTask;
    private readonly SemaphoreSlim _semaphore;

    public DelegateQueue(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _delegates = new Queue<Func<Task>>();
        _semaphore = new SemaphoreSlim(0);

        backgroundTask = Task.Run(async () => await Execute());
    }

    public async Task Enqueue(Func<Task> callback)
    {
        _delegates.Enqueue(callback);
        _semaphore.Release();
    }

    public async Task Execute()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            if (_delegates.TryDequeue(out var action))
            {
                await action();
                continue;
            }

            await _semaphore.WaitAsync();
        }
    }
}
