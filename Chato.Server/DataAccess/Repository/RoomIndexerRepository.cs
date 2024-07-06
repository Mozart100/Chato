using Chato.Server.DataAccess.Models;
using Chato.Server.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Chato.Server.DataAccess.Repository;


public class RoomIndexerDb
{

    public string RoomIndex { get; init; }

    public override int GetHashCode()
    {
        return RoomIndex.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is RoomIndexerDb entity)
        {
            return RoomIndex.Equals(entity.RoomIndex);
        }

        return false;
    }
}

public interface IRoomIndexerRepository
{
    Task AddAsync(string roomId);
    Task RemoveAsync(string roomId);
}

public class RoomIndexerRepository : IRoomIndexerRepository
{
    const int Eviction_Timeout_In_Seconds = 10;

    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

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
                        Console.WriteLine($"Cache entry with key {key} was evicted due to {reason}");
                    }
                }
            }
        };
    }

    public async Task AddAsync(string roomNameOrId)
    {
        if (_cache.TryGetValue(roomNameOrId, out _) == false)
        {
            _cache.Set(roomNameOrId, new RoomIndexerDb { RoomIndex = roomNameOrId }, _cacheEntryOptions);
        }
    }


    public async Task RemoveAsync(string roomNameOrId)
    {
        //_cache.Remove(roomNameOrId);
    }
}
