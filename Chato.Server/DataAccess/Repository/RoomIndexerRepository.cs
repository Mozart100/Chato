using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Chato.Server.DataAccess.Repository;


public record RoomIndexerDb(string RoomNameOrId);
//{

//    public string RoomIndex { get; init; }

//    public override int GetHashCode()
//    {
//        return RoomIndex.GetHashCode();
//    }

//    public override bool Equals(object? obj)
//    {
//        if (obj is RoomIndexerDb entity)
//        {
//            return RoomIndex.Equals(entity.RoomIndex);
//        }

//        return false;
//    }
//}

public interface IRoomIndexerRepository
{
    Task AddOrUpdateRoomAsync(string roomId);
    Task RemoveAsync(string roomId);
}

public class RoomIndexerRepository : IRoomIndexerRepository
{
    const int Eviction_Timeout_In_Seconds = 10;

    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    private readonly ConcurrentDictionary<object, object> _keys;


    public RoomIndexerRepository(IMemoryCache cache)
    {
        _cache = cache;
        _cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),

            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = async (key, value, reason, state) =>
                    {
                        _keys.TryAdd(key, null);
                        Console.WriteLine($"Cache entry with key {key} was evicted due to {reason}");
                    }
                }
            }
        };

        _keys = new ConcurrentDictionary<object, object>();
    }

    
    public async Task AddOrUpdateRoomAsync(string roomNameOrId)
    {
        //var room = new RoomIndexerDb(roomNameOrId);
        //using (var entry = _cache.CreateEntry(room.RoomNameOrId))
        //{
        //    entry.Value = room;
        //    entry.SetOptions(_cacheEntryOptions);
        //}
    }


    public async Task RemoveAsync(string roomNameOrId)
    {
        _cache.Remove(roomNameOrId);
    }


    public IEnumerable<object> GetAllKeys()
    {
        return _keys.Keys;
    }
}
