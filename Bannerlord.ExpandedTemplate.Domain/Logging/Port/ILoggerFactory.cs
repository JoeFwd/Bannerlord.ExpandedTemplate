namespace Bannerlord.ExpandedTemplate.Domain.Logging.Port
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger<T>();
    }
}
