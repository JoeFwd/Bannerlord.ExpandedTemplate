using System;

namespace Bannerlord.ExpandedTemplate.API.Logging;

public class LoggerAdapter(ILogger logger) : Domain.Logging.Port.ILogger
{
    public void Debug(string message, Exception? exception = null)
    {
        logger.Debug(message);
    }

    public void Info(string message, Exception? exception = null)
    {
        logger.Info(message);
    }

    public void Warn(string message, Exception? exception = null)
    {
        logger.Warn(message);
    }

    public void Error(string message, Exception? exception = null)
    {
        logger.Error(message, exception);
    }

    public void Fatal(string message, Exception? exception = null)
    {
        logger.Fatal(message, exception);
    }
}