using Microsoft.Extensions.DependencyInjection;
using Owin;
using System;


namespace Common.Tests
{
    public interface IStartup
    {
        void Configuration(IAppBuilder app);

        void AddServices(IServiceCollection services);

        void Setup(IAppBuilder app, IServiceProvider serviceProvider);
    }
}
