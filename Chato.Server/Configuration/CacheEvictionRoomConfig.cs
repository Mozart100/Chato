namespace Chato.Server.Configuration;


public interface ICacheEvictionRoomConfig
{
    int AbsoluteEviction { get; set; }
    int PeriodTimeout { get; set; }
    string TimeMeasurement { get; set; }
    int UnusedTimeout { get; set; }
}

public class CacheEvictionRoomConfigDto : ICacheEvictionRoomConfig
{
    public int PeriodTimeout { get; set; }
    public int UnusedTimeout { get; set; }
    public int AbsoluteEviction { get; set; }

    public string TimeMeasurement { get; set; }
}


public class CacheEvictionRoomConfig : PQSConfig<CacheEvictionRoomConfig>, ICacheEvictionRoomConfig
{
    public int PeriodTimeout { get; set; }
    public int UnusedTimeout { get; set; }
    public int AbsoluteEviction { get; set; }

    public string TimeMeasurement { get; set; }
}