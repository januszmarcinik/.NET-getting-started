using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NET.GettingStarted.Services;

namespace NET.GettingStarted.HealthChecks
{
    internal class TeamMembersHealthCheck : IHealthCheck
    {
        private readonly ITeamMembersService _service;

        public TeamMembersHealthCheck(ITeamMembersService service)
        {
            _service = service;
        }
        
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var isHealthy = _service.GetAll().Any();
            if (isHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Team members contains at least one element."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("There are no team members registered..."));
        }
    }
}