using Common.Tests;
using FluentAssertions;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Diagnostics.HealthChecks
{
    public class UriConsolidationHealthCheckTests: IClassFixture<UriConsolidationHealthCheckFixtures>
    {
        [Fact]
        public void UrlGroupConsolidationWithoutUnhealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisHealthy>($"http://localhost:{Port.Next()}");

            var response1 = app.Get($"{app.Url}/hc");
            var content1 = app.GetContent(response1);

            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            content1.Should().Be("Healthy");
        }

        [Fact]
        public void UrlGroupConsolidationWithUnhealthy()
        {            
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisUnhealthy>($"http://localhost:{Port.Next()}");

            var response1 = app.Get($"{app.Url}/hc");
            var content1 = app.GetContent(response1);

            response1.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            content1.Should().Be("Unhealthy");
        }

        #region [ Arrange ]

        public class StartupUrlGroupConsolidationWithUrisHealthy : StartupUrlGroupConsolidationWithUris
        {            
            public StartupUrlGroupConsolidationWithUrisHealthy() : base(
                new List<Uri> {
                    new Uri("http://localhost:12001/hc"),
                    new Uri("http://localhost:12002/hc"),
                    new Uri("http://localhost:12003/hc")
                }
            )
            {                
            }           
        }

        public class StartupUrlGroupConsolidationWithUrisUnhealthy : StartupUrlGroupConsolidationWithUris
        {
            public StartupUrlGroupConsolidationWithUrisUnhealthy() : base(
                new List<Uri> {
                    new Uri("http://localhost:12001/hc"),
                    new Uri("http://localhost:12002/hc"),
                    new Uri("http://localhost:12004/hc")
                }
            )
            {
            }
        }

        public abstract class StartupUrlGroupConsolidationWithUris : Startup
        {
            private readonly IEnumerable<Uri> _uris;
            public StartupUrlGroupConsolidationWithUris(IEnumerable<Uri> uris) : base()
            {
                _uris = uris;
            }

            public override void AddServices(IServiceCollection services)
            {
                services.AddHealthChecks()
                    .AddUrlGroupConsolidation(_uris,(results)=> {

                        if (results.Any(healthCheckResult => healthCheckResult.Status == HealthStatus.Unhealthy))
                            return HealthCheckResult.Unhealthy();

                        return HealthCheckResult.Healthy();
                    });
            }

            public override void SetupUseHealthChecks(IAppBuilder app, IServiceProvider serviceProvider)
            {
                app.UseHealthChecks(
                    serviceProvider,
                    new PathString("/hc"),
                    new Microsoft.AspNet.Diagnostics.HealthChecks.HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = (context, result) =>
                        {
                            context.Response.ContentType = "text/plain";
                            return context.Response.WriteAsync(result.Status.ToString());
                        }
                    }
                );
            }
        }     

        #endregion [ Arrange ]
    }
}
