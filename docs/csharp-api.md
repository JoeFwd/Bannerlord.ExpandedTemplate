# Bannerlord Expanded Template C# API

The **Bannerlord Expanded Template C# API** provides interfaces and classes to initialize the API with custom logging. Currently, the API is quite limited and primarily designed for initialization. If there is a demand for a more advanced API that interacts with the mod's data, we will consider implementing such use cases in future updates.

## Initializing the Expanded Template

To initialize the expanded template, use the `BannerlordExpandedTemplateApi` class. This class is responsible for bootstrapping the module and injecting any required sub-modules.

### Example:

```cs
public class MySubModule : MBSubModuleBase
{
    public MySubModule()
    {
        new BannerlordExpandedTemplateApi().Bind();
    }
}
```

## Logging API

The mod will log a number of information to help you debug issues such as empty equipment templates resulting in "nude" soldiers. By implementing a custom logger and logger factory, you can control how and where log messages are handled.

### Logger Interface (`ILogger`)

The `ILogger` interface provides methods for different log levels:

- `Debug(string message)`
- `Info(string message)`
- `Warn(string message)`
- `Error(string message, Exception? exception = null)`
- `Fatal(string message, Exception? exception = null)`

### Example: Custom Logger

You can create your own logger by implementing the `ILogger` interface:

```cs
public class ExpandedTemplateApiLogger : ILogger
{
    public void Debug(string message)
    {
        // Handle debug-level logging
        Console.WriteLine($"[DEBUG] {message}");
    }

    public void Info(string message) 
    {
        // Handle info-level logging
        Console.WriteLine($"[INFO] {message}");
    }

    public void Warn(string message) 
    {
        // Handle warnings
        Console.WriteLine($"[WARN] {message}");
    }

    public void Error(string message, Exception? exception = null) 
    {
        // Handle errors
        Console.WriteLine($"[ERROR] {message}, Exception: {exception?.Message}");
    }

    public void Fatal(string message, Exception? exception = null) 
    {
        // Handle fatal errors
        Console.WriteLine($"[FATAL] {message}, Exception: {exception?.Message}");
    }
}
```

### Logger Factory (`ILoggerFactory`)

The `ILoggerFactory` interface allows you to define a factory that creates loggers for your mod. This is useful when you want to centralize your logging logic.

### Example: Custom Logger Factory

Here is how you can implement a `LoggerFactory` to return instances of your custom logger:

```cs
public class MyLoggerFactory : ILoggerFactory
{
    public ILogger CreateLogger<T>()
    {
        return new ExpandedTemplateApiLogger(); // Return your custom logger implementation
    }
}
```

### Using a Custom Logger with the API

Once you have implemented your custom logger and logger factory, you can use them with the `BannerlordExpandedTemplateApi`.

```cs
new BannerlordExpandedTemplateApi()
    .UseLoggerFactory(new MyLoggerFactory())
    .Bind();
```

### Alternative: Logger Decorator

If you want to decorate or wrap an existing logger with additional functionality, you can use the decorator pattern. Here is an example:

```cs
public class ExpandedTemplateApiLogger : ILogger
{
    private readonly MyLogger _logger;

    public ExpandedTemplateApiLogger(MyLogger logger) 
    {
        _logger = logger;
    }

    public void Debug(string message) 
    {
        _logger.Debug(message);
    }

    public void Info(string message) 
    {
        _logger.Info(message);
    }

    public void Warn(string message) 
    {
        _logger.Warn(message);
    }

    public void Error(string message, Exception? exception = null) 
    {
        _logger.Error(message, exception);
    }

    public void Fatal(string message, Exception? exception = null) 
    {
        _logger.Fatal(message, exception);
    }
}
```
