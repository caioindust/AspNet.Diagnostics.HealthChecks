using Microsoft.Extensions.DependencyInjection;
using Owin;
using System;


namespace Common.Tests
{
    public interface IStartup
    {
        void Configuration(IAppBuilder app);

        void AddServices(IServiceCollection services);

        void SetupUseHealthChecks(IAppBuilder app, IServiceProvider serviceProvider);
    }
}
