using System.Diagnostics;

namespace Chato.Server
{

    public class LoggerStopwatch : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _leble;
        private readonly Stopwatch _stopwatch;

        public LoggerStopwatch(ILogger logger, string leble)
        {
            this._logger = logger;
            this._leble = leble;

            _stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"{_logger} started");
        }

        public LoggerStopwatch CreateSubLogger(string subLable)
        {
            subLable = subLable.Replace(" ",string.Empty).ToUpper();
            return new LoggerStopwatch(this._logger, $"{_leble} ==> {subLable}");
        }

        public void Information(string message)=> _logger.LogInformation($"{_leble} {message} [{_stopwatch.Elapsed}]");
    
        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogInformation($"{_leble} stopped [{_stopwatch.Elapsed}]");
        }
    }


    public static  class LoggerStopwatchController
    {
        private static AsyncLocal<LoggerStopwatch> _currentStopwatch = new();

        public static LoggerStopwatch Anchor (string lable , ILogger logger)
        {
            if(_currentStopwatch.Value is not null)
            {
                var current = _currentStopwatch.Value;
                var subLogger = current.CreateSubLogger(lable);
            
                _currentStopwatch.Value = subLogger;
                return subLogger;
            }

            var loggerStopwatch = new LoggerStopwatch(logger,lable);
            _currentStopwatch.Value = loggerStopwatch;
            return loggerStopwatch;


        }
    }
}
