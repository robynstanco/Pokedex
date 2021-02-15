using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pokedex.Logging.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PokedexApp
{
    public class HealthCheck : IHealthCheck
    {
        private ILoggerAdapter<HealthCheck> _logger;
        public HealthCheck(ILoggerAdapter<HealthCheck> logger)
        {
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Running " + nameof(CheckHealthAsync));

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}