namespace Bannerlord.ExpandedTemplate.API.Logging;

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message, System.Exception? exception = null);
    void Fatal(string message, System.Exception? exception = null);
}