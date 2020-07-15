using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Tests.BasicArranges
{
    public class StartupDegraded : Startup, IStartup
    {
        public StartupDegraded() : base()
        {
        }

        public override void AddServices(IServiceCollection services) =>
            services
                .AddHealthChecks()
                .AddCheck("Degraded", () => HealthCheckResult.Degraded());
    }
}
