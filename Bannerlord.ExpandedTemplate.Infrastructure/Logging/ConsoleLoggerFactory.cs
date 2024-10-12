using Bannerlord.ExpandedTemplate.Domain.Logging.Port;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Logging;

public class ConsoleLoggerFactory : ILoggerFactory
{
    public ILogger CreateLogger<T>()
    {
        return new ConsoleLogger();
    }
}
