using Common.Tests;
using FluentAssertions;
using HealthChecks.UrisConsolidation;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Sdk;

namespace Diagnostics.HealthChecks
{
    public class UriConsolidationHealthCheckTests : IClassFixture<UriConsolidationHealthCheckFixtures>, IClassFixture<HttpClientFixtures>
    {
        private readonly HttpClientFixtures _httpClientFixtures;

        public UriConsolidationHealthCheckTests(HttpClientFixtures httpClientFixtures)
        {
            _httpClientFixtures = httpClientFixtures;
        }

        [Fact]
        public void UrlGroupConsolidationIsHealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisHealthy>($"http://localhost:{Port.Next()}");

            var response = _httpClientFixtures.Get($"{app.Url}/hc");
            var content = _httpClientFixtures.GetContent(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be("Healthy");
        }

        [Fact]
        public void UrlGroupConsolidationIsUnhealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisUnhealthy>($"http://localhost:{Port.Next()}");

            var response1 = _httpClientFixtures.Get($"{app.Url}/hc");
            var content1 = _httpClientFixtures.GetContent(response1);

            response1.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            content1.Should().Be("Unhealthy");
        }

        [Fact]
        public void UrlGroupConsolidationWithNotRespondingIsUnhealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisUnhealthyNotResponding>($"http://localhost:{Port.Next()}");

            var response1 = _httpClientFixtures.Get($"{app.Url}/hc");
            var content1 = _httpClientFixtures.GetContent(response1);

            response1.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            content1.Should().Be("Unhealthy");
        }

        [Fact]
        public void UrlGroupConsolidationWithConsolidationErrorIsUnhealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithErrorConsolidation>($"http://localhost:{Port.Next()}");

            var response1 = _httpClientFixtures.Get($"{app.Url}/hc");
            var content1 = _httpClientFixtures.GetContent(response1);

            response1.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            content1.Should().Be("Unhealthy");
        }

        [Fact]
        public void UrlGroupConsolidationWithPostIsHealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisHealthyAndPost>($"http://localhost:{Port.Next()}");

            var response = _httpClientFixtures.Get($"{app.Url}/hc");
            var content = _httpClientFixtures.GetContent(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be("Healthy");
        }

        [Fact]
        public void UrlGroupConsolidationWithUrisOptionsIsUnhealthy()
        {
            using var app = new AppBuilder<StartupUrlGroupConsolidationWithUrisOptionsHealthy>($"http://localhost:{Port.Next()}");

            var response = _httpClientFixtures.Get($"{app.Url}/hc");
            var content = _httpClientFixtures.GetContent(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be("Healthy");
        }

        #region [ Arrange ]

        public class StartupUrlGroupConsolidationWithUrisHealthy : StartupUrlGroupConsolidationWithUris
        {
            public StartupUrlGroupConsolidationWithUrisHealthy() : base(
                new List<Uri> {
                    new Uri("http://localhost:12001"),
                    new Uri("http://localhost:12002"),
                    new Uri("http://localhost:12003")
                }
            )
            {
            }
        }

        public class StartupUrlGroupConsolidationWithUrisUnhealthy : StartupUrlGroupConsolidationWithUris
        {
            public StartupUrlGroupConsolidationWithUrisUnhealthy() : base(
                new List<Uri> {
                    new Uri("http://localhost:12001"),
                    new Uri("http://localhost:12002"),
                    new Uri("http://localhost:12003/failure")
                }
            )
            {
            }
        }

        public class StartupUrlGroupConsolidationWithUrisUnhealthyNotResponding : StartupUrlGroupConsolidationWithUris
        {
            public StartupUrlGroupConsolidationWithUrisUnhealthyNotResponding() : base(
                new List<Uri> {
                    new Uri("http://localhost:12001"),
                    new Uri("http://localhost:12002"),
                    new Uri("http://localhost:12004")
                }
            )
            {
            }
        }

        public class StartupUrlGroupConsolidationWithErrorConsolidation : StartupUrlGroupConsolidationWithUrisHealthy
        {
            public StartupUrlGroupConsolidationWithErrorConsolidation() : base()
            {
                SetConsolidation((results) =>
                {
                    throw new Exception("Test consolidation exception");
                });
            }
        }

        public class StartupUrlGroupConsolidationWithUrisHealthyAndPost : StartupUrlGroupConsolidationWithUrisHealthy
        {
            public StartupUrlGroupConsolidationWithUrisHealthyAndPost() : base()
            {
                SetHttpMethod(HttpMethod.Post);
            }
        }

        public class StartupUrlGroupConsolidationWithUrisOptionsHealthy : StartupUrlGroupConsolidationWithUrisOptions
        {
            public StartupUrlGroupConsolidationWithUrisOptionsHealthy() : base(
                (opt) => {
                    opt
                        .AddUri(new Uri("http://localhost:12001"), (setup) =>
                        {
                            setup
                                .AddCustomHeader("header-teste", "teste")
                                .UseGet()
                                .UseTimeout(TimeSpan.FromSeconds(5));
                        })
                        .AddUri(new Uri("http://localhost:12002"), (setup) =>
                        {
                            setup
                                .AddCustomHeader("header-teste", "teste")
                                .UsePost()
                                .UseTimeout(TimeSpan.FromSeconds(5));
                        }); ;
                }
            )
            {
            }
        }
        
        public abstract class StartupUrlGroupConsolidationWithUris : StartupUrlGroupConsolidation
        {
            private readonly IEnumerable<Uri> _uris;

            private HttpMethod _httpMethod = null;

            private Func<IEnumerable<HealthCheckResult>, HealthCheckResult> _consolidation = (results) =>
            {
                if (results.Any(healthCheckResult => healthCheckResult.Status == HealthStatus.Unhealthy))
                    return HealthCheckResult.Unhealthy();

                return HealthCheckResult.Healthy();
            };

            public StartupUrlGroupConsolidationWithUris(IEnumerable<Uri> uris) : base()
            {
                _uris = uris;
            }

            protected void SetConsolidation(Func<IEnumerable<HealthCheckResult>, HealthCheckResult> consolidation)
                => _consolidation = consolidation;

            protected void SetHttpMethod(HttpMethod httpMethod)
                => _httpMethod = httpMethod;

            public override void AddServices(IServiceCollection services)
            {
                if (_httpMethod == null)
                    services.AddHealthChecks()
                        .AddUrlGroupConsolidation(_uris, _consolidation);
                else
                    services.AddHealthChecks()
                        .AddUrlGroupConsolidation(_uris, _consolidation, _httpMethod);
            }
        }

        public abstract class StartupUrlGroupConsolidationWithUrisOptions : StartupUrlGroupConsolidation
        {
            private readonly Action<UrisConsolidationHealthCheckOptions> _uriOptions;

            private HttpMethod _httpMethod = null;
            
            public StartupUrlGroupConsolidationWithUrisOptions(Action<UrisConsolidationHealthCheckOptions> uriOptions) : base()
            {
                _uriOptions = uriOptions;
            }

            protected void SetHttpMethod(HttpMethod httpMethod)
                => _httpMethod = httpMethod;

            public override void AddServices(IServiceCollection services)
            {
                if (_httpMethod == null)
                    services.AddHealthChecks()
                        .AddUrlGroupConsolidation(_uriOptions, (results) =>
                        {
                            if (results.Any(healthCheckResult => healthCheckResult.Status == HealthStatus.Unhealthy))
                                return HealthCheckResult.Unhealthy();

                            return HealthCheckResult.Healthy();
                        });
            }
        }

        public abstract class StartupUrlGroupConsolidation : Startup
        {
            public StartupUrlGroupConsolidation() : base()
            {  
            }

            public override void Setup(IAppBuilder app, IServiceProvider serviceProvider)
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
