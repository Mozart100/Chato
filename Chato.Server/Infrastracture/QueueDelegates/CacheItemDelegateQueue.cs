using Chato.Server.DataAccess.Repository;

namespace Chato.Server.Infrastracture.QueueDelegates;

public interface ICacheItemDelegateQueue 
{
    Task<bool> ToRemoveAsync(string roomNameOrId);
    Task<RoomIndexerDb> PopOrWaitAsync(CancellationToken cancellationToken);

}

public class CacheItemDelegateQueue : DelegateQueueBase<RoomIndexerDb>, ICacheItemDelegateQueue
{
    public override Task InvokeAsync(RoomIndexerDb callback)
    {
        return base.InvokeAsync(callback);
    }

    public override Task<RoomIndexerDb> PopOrWaitAsync(CancellationToken cancellationToken)
    {
        return base.PopOrWaitAsync(cancellationToken);
    }

    public async Task<bool> ToRemoveAsync(string roomNameOrId)
    {
        return true;
    }
}

