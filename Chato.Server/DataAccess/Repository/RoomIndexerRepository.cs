using Chato.Server.Configuration;
using Chato.Server.Infrastracture;
using Chato.Server.Infrastracture.QueueDelegates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Chato.Server.DataAccess.Repository;


public class RoomIndexerCache
{
    public string RoomNameOrId { get; set; }
}


public interface IRoomIndexerRepository
{
    void AddToCache(string roomNameOrId);
    //Task AddToCacheAsync(string roomId)
    void Remove(string roomNameOrId);

    IEnumerable<(string Key, TimeOnly UnusedTimeStamp, TimeOnly ThreshholdAbsoluteEviction )> GetAllKeyValuesSnapshot();


    //Task RemoveAsync(string roomId);
}

public class RoomIndexerRepository : IRoomIndexerRepository
{
    private readonly ConcurrentDictionary<string, TimeOnly> _roomTimeStemps;
    private readonly ConcurrentDictionary<string, TimeOnly> _roomAbsoluteEviction;
    private readonly CacheEvictionRoomConfig _config;

    public RoomIndexerRepository(IOptions<CacheEvictionRoomConfig> config)
    {
        _roomTimeStemps = new ConcurrentDictionary<string, TimeOnly>();
        _roomAbsoluteEviction = new ConcurrentDictionary<string, TimeOnly>();
        _config = config.Value;
    }

    public void AddToCache(string roomNameOrId)
    {
        _roomTimeStemps.AddOrUpdate(roomNameOrId, key =>
        {
            var timeStemp = TimeOnly.FromDateTime(DateTime.UtcNow);

            var timeSpan = TimeSpan.FromSeconds(_config.AbsoluteEviction);

            _roomAbsoluteEviction.GetOrAdd(roomNameOrId, timeStemp.Add(timeSpan));
            return timeStemp;
        },
        
        (key, currentTimeStemp) =>
        {

            var time = TimeOnly.FromDateTime(DateTime.UtcNow);
            Console.WriteLine($"Updating timestamp for room '{key}': Minute = {time.Minute}  Scecond = {time.Second} and MilliSecond {time.Millisecond}");
            return time;
        }
            );
    }

    public void Remove(string roomNameOrId)
    {
        _roomTimeStemps.Remove(roomNameOrId, out _);
        _roomAbsoluteEviction.Remove(roomNameOrId, out _);
    }

    public IEnumerable<(string Key, TimeOnly UnusedTimeStamp, TimeOnly ThreshholdAbsoluteEviction)> GetAllKeyValuesSnapshot()
    {
        var snapshot = new Dictionary<string, TimeOnly>(_roomTimeStemps);
        var list = new List<(string key, TimeOnly value, TimeOnly ThreshholdAbsoluteEviction)>();

        foreach (var kvp in snapshot)
        {
            //_roomAbsoluteEviction.TryGetValue(kvp.Key, out var )
            list.Add((kvp.Key, kvp.Value, _roomAbsoluteEviction[kvp.Key]));
        }

        return list;
    }
}

//public class RoomIndexerRepository : IRoomIndexerRepository
//{
//    const int Eviction_Timeout_In_Seconds = 5;

//    private readonly IMemoryCache _cache;
//    private readonly ICacheItemDelegateQueue _cacheItemDelegateQueue;

//    //private readonly IRoomService _roomService;
//    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

//    private readonly ConcurrentDictionary<string, object> _keys;


//    public RoomIndexerRepository(IMemoryCache cache, ICacheItemDelegateQueue cacheItemDelegateQueue)
//    {
//        _cache = cache;
//        this._cacheItemDelegateQueue = cacheItemDelegateQueue;

//        //_cacheEntryOptions = new MemoryCacheEntryOptions()
//        //{
//        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
//        //    SlidingExpiration = TimeSpan.FromSeconds(4),

//        //};

//        //_cacheEntryOptions.RegisterPostEvictionCallback(PostEvictionCallback);

//        _keys = new ConcurrentDictionary<string, object>();
//    }


//    public async Task AddToCacheAsync(string roomNameOrId)
//    {
//        AddToCache(roomNameOrId);
//    }

//     void PostEvictionCallback(object key, object value, EvictionReason evictionReason, object state)
//    {
//        if(evictionReason == EvictionReason.Expired)
//        {
//            if( value is RoomIndexerCache data)
//            {
//                //await _cacheItemDelegateQueue.EnqueueAsync(data);
//            }
//        }
//    }

//    public void AddToCache(string roomNameOrId)
//    {
//        //var cacheEntryOptions = new MemoryCacheEntryOptions()l
//        //{
//        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
//        //    SlidingExpiration = TimeSpan.FromSeconds(1),

//        //};

//        //cacheEntryOptions.RegisterPostEvictionCallback(PostEvictionCallback);

//        _cache.GetOrCreate(roomNameOrId, entry =>
//        {
//            entry.SlidingExpiration =  TimeSpan.FromSeconds(2);
//            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(6);
//            entry.RegisterPostEvictionCallback(PostEvictionCallback);
//            entry.SetPriority(CacheItemPriority.High);
//            return new RoomIndexerCache { RoomNameOrId = roomNameOrId };
//        });

//        //_cache.GetOrCreate(roomNameOrId, entry =>
//        //{
//        //    entry.SlidingExpiration = null;// TimeSpan.FromSeconds(2);
//        //    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2);
//        //    entry.RegisterPostEvictionCallback(PostEvictionCallback);
//        //    entry.SetPriority(CacheItemPriority.Low);
//        //    return new RoomIndexerCache { RoomNameOrId = roomNameOrId };
//        //});

//        //var tmp = _cache.Get<RoomIndexerCache>(roomNameOrId);

//        //if (_cache.TryGetValue(roomNameOrId,out var item) == false)
//        //{

//        //    _cache.Set(roomNameOrId, new RoomIndexerCache { RoomNameOrId = roomNameOrId }, cacheEntryOptions);

//        //}

//        //using (var entry = _cache.CreateEntry(room.RoomNameOrId))
//        //{
//        //    entry.Value = room;
//        //    entry.SetOptions(_cacheEntryOptions);
//        //    _keys.TryAdd(room.RoomNameOrId, null);
//        //}
//    }


//    public async Task RemoveAsync(string roomNameOrId)
//    {
//        Remove(roomNameOrId);
//    }

//    public void Remove(string roomNameOrId)
//    {
//        //_cache.Remove(roomNameOrId);
//        //_keys.TryRemove(roomNameOrId, out _);

//    }

//    public IEnumerable<string> GetAllKeys()
//    {
//        return _keys.Keys;
//    }
//}
