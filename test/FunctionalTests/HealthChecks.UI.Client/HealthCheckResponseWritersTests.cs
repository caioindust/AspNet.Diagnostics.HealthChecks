using AspNet.HealthChecks.UI.Client;
using Common.Tests;
using FluentAssertions;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Net;
using Xunit;

namespace Diagnostics.HealthChecks
{    
    public class HealthCheckResponseWritersTests: IClassFixture<HttpClientFixtures>
    {
        private readonly HttpClientFixtures _httpClientFixtures;

        public HealthCheckResponseWritersTests(HttpClientFixtures httpClientFixtures)
        {
            _httpClientFixtures = httpClientFixtures;
        }

        [Fact]
        public void HealthChecksWithhHealthCheckOptions()
        {
            using var app = new AppBuilder<StartupWithhHealthCheckOptions>($"http://localhost:{Port.Next()}");
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            var response = _httpClientFixtures.Get($"{app.Url}/hc");
            var content = _httpClientFixtures.GetContent(response);

            var uiReport =
                response.StatusCode.Equals(HttpStatusCode.OK) ?
                    JsonConvert.DeserializeObject<UIHealthReport>(content, settings) :
                    null;

            uiReport.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            uiReport.Status.Should().Be(UIHealthStatus.Healthy);
            uiReport.Entries.Should().NotBeEmpty();
        }

        #region [ Arrange ]

        public class StartupWithhHealthCheckOptions : Startup
        {
            public StartupWithhHealthCheckOptions()
                : base() { }

            public override void AddServices(IServiceCollection services)
            {
                services.AddHealthChecks()
                    .AddCheck("Healthy", () =>
                    {
                        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
                    }).AddCheck("Healthy2", () =>
                    {
                        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
                    });
            }

            public override void Setup(IAppBuilder app, IServiceProvider serviceProvider)
            {
                app.UseHealthChecks(
                    serviceProvider,
                    new PathString("/hc"),
                    new Microsoft.AspNet.Diagnostics.HealthChecks.HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    }
                );
            }
        }

        #endregion [ Arrange ]
    }
}
