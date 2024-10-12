using System;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Debug(string message, System.Exception? exception = null)
        {
            Log("Debug", message, exception);
        }

        public void Info(string message, System.Exception? exception = null)
        {
            Log("Info", message, exception);
        }

        public void Warn(string message, System.Exception? exception = null)
        {
            Log("Warn", message, exception);
        }

        public void Error(string message, System.Exception? exception = null)
        {
            Log("Error", message, exception);
        }

        public void Fatal(string message, System.Exception? exception = null)
        {
            Log("Fatal", message, exception);
        }

        private void Log(string level, string message, System.Exception? exception = null)
        {
            if (exception is null)
                Console.Out.WriteLine($"{FormatLogLevel(level)}{message}");
            else
                Console.Out.WriteLine($"{FormatLogLevel(level)}{message}\nException:{exception}");
        }

        private string FormatLogLevel(string level)
        {
            return $"[{level}]";
        }
    }
}
