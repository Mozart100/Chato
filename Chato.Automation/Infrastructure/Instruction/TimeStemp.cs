namespace Chato.Automation.Infrastructure.Instruction;


public struct TimeStemp
{
    private int _start;

    public void Start()
    {
        _start = Environment.TickCount;
    }

    public int StartTimestamp
    {
        get { return _start; }
    }

    public int Elapsed
    {
        get { return Environment.TickCount - _start; }
    }

    public static TimeStemp Reset()
    {
        return new TimeStemp();
    }


    public bool IsSecondOver(int seconds)
    {
        var curr = Environment.TickCount;
        var millisecondsDifference = curr - _start;

        double secondsDifference = millisecondsDifference / 1000.0;
        return secondsDifference >= seconds; // Use >= to include the exact second.
    }
}

