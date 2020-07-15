using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Owin;
using System;

namespace Common.Tests
{
    public abstract class Startup : IStartup
    {
        private readonly IServiceCollection _services;

        protected Startup()
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

        public virtual void SetupUseHealthChecks(IAppBuilder app, IServiceProvider serviceProvider)
        {
            app.UseHealthChecks(serviceProvider, new PathString("/hc"));
        }
    }
}
