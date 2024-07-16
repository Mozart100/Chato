﻿namespace Chato.Server.Configuration;

public class PQSConfig<TClass> where TClass : class
{
    public static string ApiName => typeof(TClass).Name;

    public string ConfigurationName => ApiName;
}

public class CacheEvictionRoomConfig : PQSConfig<CacheEvictionRoomConfig>
{
    public int PeriodTimeoutSeconds { get; set; }
    public int UnusedTimeoutSeconds { get; set; }
    public int AbsoluteEvictionInSeconds { get; set; }
}