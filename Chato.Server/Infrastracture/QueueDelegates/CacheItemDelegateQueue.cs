using Chato.Server.DataAccess.Repository;

namespace Chato.Server.Infrastracture.QueueDelegates;

public interface ICacheItemDelegateQueue 
{
    Task<bool> ToRemoveAsync(string roomNameOrId);
    Task<RoomIndexerCache> PopOrWaitAsync(CancellationToken cancellationToken);
    Task EnqueueAsync(string roomName);

}

public class CacheItemDelegateQueue : DelegateQueueBase<RoomIndexerCache>, ICacheItemDelegateQueue
{
    public async Task EnqueueAsync(string roomName)
    {
        await InvokeAsync(new RoomIndexerCache(roomName));
    }

    public override Task<RoomIndexerCache> PopOrWaitAsync(CancellationToken cancellationToken)
    {
        return base.PopOrWaitAsync(cancellationToken);
    }

    public async Task<bool> ToRemoveAsync(string roomNameOrId)
    {
        return true;
    }
}

