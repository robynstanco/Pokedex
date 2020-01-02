using System;

namespace Pokedex.Logging.Interfaces
{
    public interface ILoggerAdapter<T>
    {
        void LogCritical(Exception ex, string message, params object[] args);
        void LogDebug(Exception ex, string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        void LogInformation(string message);
        void LogTrace(Exception ex, string message, params object[] args);
        void LogWarning(Exception ex, string message, params object[] args);
    }
}