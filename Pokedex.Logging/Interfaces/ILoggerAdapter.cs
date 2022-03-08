using System;

namespace Pokedex.Logging.Interfaces
{
    public interface ILoggerAdapter<T>
    {
        /// <summary>
        /// Log a critical message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        void LogCritical(Exception ex, string message, params object[] args);

        /// <summary>
        /// Log a debug message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        void LogDebug(Exception ex, string message, params object[] args);

        /// <summary>
        /// Log an error message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        void LogError(Exception ex, string message, params object[] args);

        /// <summary>
        /// Log a simple informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogInformation(string message);

        /// <summary>
        /// Log a trace message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        void LogTrace(Exception ex, string message, params object[] args);

        /// <summary>
        /// Log a warning message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        void LogWarning(Exception ex, string message, params object[] args);
    }
}