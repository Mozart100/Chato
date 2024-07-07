namespace Chato.Server.Infrastracture.QueueDelegates;


public interface ILockerDelegateQueue : IDelegateQueue;

public class LockerDelegateQueue : DelegateQueueBase, ILockerDelegateQueue
{

}

