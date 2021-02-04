using Microsoft.Extensions.Logging;
using Pokedex.Logging.Interfaces;
using System;

namespace Pokedex.Logging.Adapters
{
    //todo, unit test logging adapter
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;
        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogCritical(Exception ex, string message, params object[] args)
        {
            _logger.LogCritical(ex, message, args);
        }

        public void LogDebug(Exception ex, string message, params object[] args)
        {
            _logger.LogDebug(ex, message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogTrace(Exception ex, string message, params object[] args)
        {
            _logger.LogTrace(ex, message, args);
        }

        public void LogWarning(Exception ex, string message, params object[] args)
        {
            _logger.LogWarning(ex, message, args);
        }
    }
}