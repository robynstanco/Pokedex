using Microsoft.Extensions.Logging;
using Pokedex.Logging.Interfaces;
using System;

namespace Pokedex.Logging.Adapters
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;
        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Log a critical message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        public void LogCritical(Exception ex, string message, params object[] args)
        {
            _logger.LogCritical(ex, message, args);
        }

        /// <summary>
        /// Log a debug message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        public void LogDebug(Exception ex, string message, params object[] args)
        {
            _logger.LogDebug(ex, message, args);
        }

        /// <summary>
        /// Log an error message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        /// <summary>
        /// Log a simple informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        /// <summary>
        /// Log a trace message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        public void LogTrace(Exception ex, string message, params object[] args)
        {
            _logger.LogTrace(ex, message, args);
        }

        /// <summary>
        /// Log a warning message with exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional params, if any.</param>
        public void LogWarning(Exception ex, string message, params object[] args)
        {
            _logger.LogWarning(ex, message, args);
        }
    }
}