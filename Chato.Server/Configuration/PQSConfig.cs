namespace Chato.Server.Configuration;

public class PQSConfig<TClass> where TClass : class
{
    public static string ApiName => typeof(TClass).Name;

    public string ConfigurationName => ApiName;
}

public class CacheEvictionRoomConfig : PQSConfig<CacheEvictionRoomConfig>
{
    public int PeriodTimeout { get; set; }
    public int UnusedTimeout { get; set; }
    public int AbsoluteEviction { get; set; }
}