namespace Chato.Server.Infrastracture.QueueDelegates;

public interface IDelegateQueue
{
    Task BeginInvokeAsync(Func<Task> callback);
    void Invoke(Action callback);
    Task InvokeAsync(Func<Task> callback);
    Task<Func<Task>> PopOrWaitAsync(CancellationToken cancellationToken);
}

public class LockDelegateQueue : DelegateQueueBase, IDelegateQueue
{

}



//public class DelegateQueue : DelegateQueueBase, IDelegateQueue
//{

//}

