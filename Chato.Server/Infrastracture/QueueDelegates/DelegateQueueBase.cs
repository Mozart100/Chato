namespace Chato.Server.Infrastracture.QueueDelegates;

public interface IDelegateQueue
{
    Task BeginInvokeAsync(Func<Task> callback);
    void Invoke(Action callback);
    Task InvokeAsync(Func<Task> callback);
    Task<Func<Task>> PopOrWaitAsync(CancellationToken cancellationToken);
}

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


public abstract class DelegateQueueBase<TInstance> where TInstance : class
{
    private readonly Queue<Func<Task<TInstance>>> _delegates;
    private readonly SemaphoreSlim _semaphore;


    public DelegateQueueBase()
    {
        _delegates = new Queue<Func<Task<TInstance>>>();
        _semaphore = new SemaphoreSlim(0);

    }

    public async Task BeginInvokeAsync(TInstance instance)
    {
        Func<Task<TInstance>> callback = async () => instance;
        Enqueue(callback);
    }

    public virtual async Task InvokeAsync(TInstance instance)
    {
        var manuelResetEvent = new ManualResetEvent(false);

        Func<Task<TInstance>> callback = async () => instance;
        Func<Task<TInstance>> wrapper = async () =>
        {
            var result = await callback();
            manuelResetEvent.Set();
            return result;
        };

        if (Enqueue(wrapper))
        {
            manuelResetEvent.WaitOne();
        }
    }


    //    Func<Task<TInstance>> wrapper = async () =>
    //    {
    //        callback();
    //        manuelResetEvent.Set();
    //    };

    //    if (Enqueue(wrapper))
    //    {
    //        manuelResetEvent.WaitOne();
    //    }
    //}

    private bool Enqueue(Func<Task<TInstance>> callback)
    {
        if (callback == null)
        {
            return false;
        }

        _delegates.Enqueue(callback);
        _semaphore.Release();

        return true;
    }

    public virtual async Task<TInstance> PopOrWaitAsync(CancellationToken cancellationToken)
    {
        if (_delegates.TryDequeue(out var action))
        {
            return await action();
        }

        await _semaphore.WaitAsync(cancellationToken);
        return null;
    }
}

