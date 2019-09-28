using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Owin;
using System;

namespace Common.Tests
{
    public abstract class Startup
    {
        private readonly IServiceCollection _services;
        public Startup()
        {
            _services = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole());
        }

        public void Configuration(IAppBuilder app)
        {
            AddServices(_services);
            SetupUseHealthChecks(app, _services.BuildServiceProvider());
        }

        public abstract void AddServices(IServiceCollection services);
        public virtual void SetupUseHealthChecks(IAppBuilder app, IServiceProvider servicesProvider)
        {
            app.UseHealthChecks(servicesProvider, new PathString("/hc"));
        }
    }
}
