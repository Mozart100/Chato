namespace Chato.Server.Infrastracture;


public struct TimeStamp
{
    private readonly int _start;

    private TimeStamp(int start)
    {
        _start = start;
    }

    public static TimeStamp Reset()
    {
        return new TimeStamp(Environment.TickCount);
    }
    public int Elapsed
    {
        get { return Environment.TickCount - _start; }
    }

    public bool IsSecondOver(int seconds)
    {
        var curr = Environment.TickCount;
        var millisecondsDifference = curr - _start;

        double secondsDifference = millisecondsDifference / 1000.0;
        return secondsDifference >= seconds; // Use >= to include the exact second.
    }
}

