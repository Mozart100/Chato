namespace Chato.Server.Infrastracture;

public interface IDelegateQueue
{
    Task BeginInvokeAsync(Func<Task> callback);
    void Initialize(CancellationToken stoppingToken);
    void Invoke(Action callback);
    Task InvokeAsync(Func<Task> callback);
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

    public async Task BeginInvokeAsync(Func<Task> callback)
    {
        Enqueue(callback);
    }

    public async Task InvokeAsync(Func<Task> callback)
    {
        var manuelResetEvent = new ManualResetEvent(false);

        Func<Task> wrapper = async () =>
        {
            await callback();
            manuelResetEvent.Set();
        };

        if (Enqueue(wrapper))
        {
            manuelResetEvent.WaitOne();
        }
    }



    public void Invoke(Action callback)
    {
        var manuelResetEvent = new ManualResetEvent(false);

        Func<Task> wrapper = async () =>
        {
            callback();
            manuelResetEvent.Set();
        };

        if (Enqueue(wrapper))
        {
            manuelResetEvent.WaitOne();
        }
    }

    private bool Enqueue(Func<Task> callback)
    {
        if (callback == null)
        {
            return false;
        }

        _delegates.Enqueue(callback);
        _semaphore.Release();

        return true;
    }

    private async Task Execute()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            if (_delegates.TryDequeue(out var action))
            {
                if (action == null)
                {
                    continue;
                }
                if (action != null)
                {
                    await action();
                }

                continue;
            }

            await _semaphore.WaitAsync();
        }
    }
}
