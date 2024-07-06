namespace Chato.Server.Infrastracture.QueueDelegates;

public abstract class DelegateQueueBase 
{
    private readonly Queue<Func<Task>> _delegates;
    private readonly SemaphoreSlim _semaphore;


    public DelegateQueueBase()
    {
        _delegates = new Queue<Func<Task>>();
        _semaphore = new SemaphoreSlim(0);

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

    public async Task<Func<Task>> PopOrWaitAsync(CancellationToken cancellationToken)
    {
        if (_delegates.TryDequeue(out var action))
        {
            return action;
        }

        await _semaphore.WaitAsync(cancellationToken);

        return null;
    }
}

