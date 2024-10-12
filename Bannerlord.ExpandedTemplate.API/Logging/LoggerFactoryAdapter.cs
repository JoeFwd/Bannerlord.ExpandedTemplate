namespace Bannerlord.ExpandedTemplate.API.Logging;

internal class LoggerFactoryAdapter(ILoggerFactory loggerFactory) : Domain.Logging.Port.ILoggerFactory
{
    public Domain.Logging.Port.ILogger CreateLogger<T>()
    {
        return new LoggerAdapter(loggerFactory.CreateLogger<T>());
    }
}