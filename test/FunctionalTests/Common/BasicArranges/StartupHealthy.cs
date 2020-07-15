using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Tests.BasicArranges
{
    public class StartupHealthy : Startup, IStartup
    {
        public StartupHealthy() : base()
        {
        }

        public override void AddServices(IServiceCollection services) =>
            services
                .AddHealthChecks()
                .AddCheck("Healthy", () => HealthCheckResult.Healthy());
    }
}
