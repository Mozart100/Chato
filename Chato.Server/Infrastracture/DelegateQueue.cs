namespace Chato.Server.Infrastracture;

public interface IDelegateQueue
{
    Task EnqueueAsync(Func<Task> callback);
    void Initialize(CancellationToken stoppingToken);
}

public class DelegateQueue : IDelegateQueue
{
    private readonly Queue<Func<Task>> _delegates;
    private readonly SemaphoreSlim _semaphore;

    private CancellationToken _cancellationToken;
    private Task _backgroundTask;

    public DelegateQueue()
    {
        _delegates = new Queue<Func<Task>>();
        _semaphore = new SemaphoreSlim(0);

    }

    public void Initialize(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        _backgroundTask = Task.Run(async () => await Execute());
    }

    public async Task EnqueueAsync(Func<Task> callback)
    {
        _delegates.Enqueue(callback);
        _semaphore.Release();
    }

    public void Enqueue(Action callback)
    {
        var manuelResetEvent = new ManualResetEvent(false);

        Func<Task> wrapper = async () =>
        {
            callback();
            manuelResetEvent.Set();
        };

        _delegates.Enqueue(wrapper);
        _semaphore.Release();

        manuelResetEvent.WaitOne();
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
