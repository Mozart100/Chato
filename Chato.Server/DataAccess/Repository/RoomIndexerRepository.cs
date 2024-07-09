using Chato.Server.DataAccess.Models;
using Chato.Server.Infrastracture.QueueDelegates;
using Chato.Server.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Chato.Server.DataAccess.Repository;


public record RoomIndexerCache(string RoomNameOrId);

public interface IRoomIndexerRepository
{
    void AddOrUpdateRoom(string roomNameOrId);
    Task AddOrUpdateRoomAsync(string roomId);
    void Remove(string roomNameOrId);
    Task RemoveAsync(string roomId);
}

public class RoomIndexerRepository : IRoomIndexerRepository
{
    const int Eviction_Timeout_In_Seconds = 10;

    private readonly IMemoryCache _cache;
    private readonly ICacheItemDelegateQueue _cacheItemDelegateQueue;

    //private readonly IRoomService _roomService;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    private readonly ConcurrentDictionary<string, object> _keys;


    public RoomIndexerRepository(IMemoryCache cache, ICacheItemDelegateQueue cacheItemDelegateQueue)
    {
        _cache = cache;
        this._cacheItemDelegateQueue = cacheItemDelegateQueue;

        _cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(Eviction_Timeout_In_Seconds),

            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = async (key, value, reason, state) =>
                    {
                       if( key is string nameOrId )
                       {
                            await _cacheItemDelegateQueue.EnqueueAsync(nameOrId);
                             _keys.TryRemove(nameOrId, out _);
                       }

                        Console.WriteLine($"Cache entry with key {key} was evicted due to {reason}");
                    }
                }
            }
        };

        _keys = new ConcurrentDictionary<string, object>();
    }


    public async Task AddOrUpdateRoomAsync(string roomNameOrId)
    {
        AddOrUpdateRoom(roomNameOrId);
    }

    public void AddOrUpdateRoom(string roomNameOrId)
    {
        var room = new RoomIndexerCache(roomNameOrId);
        using (var entry = _cache.CreateEntry(room.RoomNameOrId))
        {
            entry.Value = room;
            entry.SetOptions(_cacheEntryOptions);
            _keys.TryAdd(room.RoomNameOrId, null);
        }
    }

    public async Task RemoveAsync(string roomNameOrId)
    {
        Remove(roomNameOrId);
    }

    public void Remove(string roomNameOrId)
    {
        _keys.TryRemove(roomNameOrId, out _);
        _cache.Remove(roomNameOrId);
    }



    public IEnumerable<string> GetAllKeys()
    {
        return _keys.Keys;
    }
}
