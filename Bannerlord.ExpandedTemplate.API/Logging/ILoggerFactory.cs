namespace Bannerlord.ExpandedTemplate.API.Logging;

public interface ILoggerFactory
{
    ILogger CreateLogger<T>();
}