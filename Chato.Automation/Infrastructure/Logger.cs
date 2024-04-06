using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chato.Automation.Infrastructure
{
    public interface IAutomationLogger
    {
        void Info(string message);
    }

    internal class ConsoleLogger : IAutomationLogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }



    public class AutomationLoggerFactory
    {
        public static IAutomationLogger CreateLogger()
        {
            return new ConsoleLogger();
        }
    }
}
