namespace Chato.Server.Services;

public static class CacheEvictionUtility
{
    public static TimeOnly ConvertToTimeOnly(string timeMeasurement, int target)
    {
        timeMeasurement = timeMeasurement.ToLower();

        var time = default(TimeOnly);
        if (timeMeasurement == "hour")
        {
            time = TimeOnly.FromTimeSpan(TimeSpan.FromHours(target));
        }
        else
        {
            if (timeMeasurement == "minute")
            {
                time = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(target));
            }
            else
            {
                //if (timeMeasurement == "second")
                {
                    time = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(target));
                }
            }
        }

        return time;
    }

    public static TimeSpan ConvertToTimeSpan(string timeMeasurement, int target)
    {
        timeMeasurement = timeMeasurement.ToLower();

        var time = default(TimeSpan);
        if (timeMeasurement == "hour")
        {
            time = TimeSpan.FromHours(target);
        }
        else
        {
            if (timeMeasurement == "minute")
            {
                time = TimeSpan.FromMinutes(target);
            }
            else
            {
                //if (timeMeasurement == "second")
                {
                    time = TimeSpan.FromSeconds(target);
                }
            }
        }

        return time;
    }


    public static TimeOnly Add(string timeMeasurement, TimeOnly current, int target)
    {
        var time = ConvertToTimeOnly(timeMeasurement, target);
        return current.Add(time.ToTimeSpan());
    }
}
