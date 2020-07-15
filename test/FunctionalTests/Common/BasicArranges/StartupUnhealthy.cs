using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Tests.BasicArranges
{
    public class StartupUnhealthy : Startup, IStartup
    {
        public StartupUnhealthy() : base()
        {
        }

        public override void AddServices(IServiceCollection services) =>
            services
                .AddHealthChecks()
                .AddCheck("Unhealthy", () => HealthCheckResult.Unhealthy());
    }
}
