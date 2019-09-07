using Common.Tests;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Xunit;
using FluentAssertions;

namespace Diagnostics.HelthChecks
{
    public class HealthCheckApplicationBuilderExtensionsTests
    {
        [Theory]
        [ClassData(typeof(HealthChecksTestData))]
        public void HealthChecks<T>(AppBuilder<T> app, string expected)
        {
            var contents = app.GetContent($"{app.Url}/hc");            
            contents.Should().Be(expected);
        }

        [Fact]
        public void HealthChecksWithHealthCheckOptions()
        {
            using (var app = new AppBuilder<StartupWithhHealthCheckOptions>($"http://localhost:{Port.Next()}"))
            {
                var response1 = app.Get($"{app.Url}/hc-tag-predicate1");
                var content1 = app.GetContent(response1);

                var response2 = app.Get($"{app.Url}/hc-tag-predicate2");
                var content2 = app.GetContent(response2);
                
                response1.StatusCode.Should().Be(HttpStatusCode.OK);
                content1.Should().Be("Healthy");

                response2.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
                content2.Should().Be("Unhealthy");
            }
        }

        [Fact]
        public void HealthChecksWithPort()
        {
            using (var app = new AppBuilder<StartupWithPort>($"http://localhost:12345"))
            {
                var response = app.Get($"{app.Url}/hc");
                var content = app.GetContent(response);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                content.Should().Be("Healthy");
            }
        }

        [Fact]
        public void HealthCheckApplicationBuilderExtensionsWithAppNull()
        {
            FluentActions.Invoking(() =>
                HealthCheckApplicationBuilderExtensions.UseHealthChecks(null, null, new PathString(""))
            )
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void HealthCheckApplicationBuilderExtensionsWithAppAndOptionsNull()
        {
            FluentActions.Invoking(() =>
                HealthCheckApplicationBuilderExtensions.UseHealthChecks(null, null, new PathString(""), options: null)
            )
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void HealthCheckApplicationBuilderExtensionsWithAppNullAndPort()
        {
            FluentActions.Invoking(() =>
                HealthCheckApplicationBuilderExtensions.UseHealthChecks(null, null, new PathString(""), 25)
            )
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void HealthCheckApplicationBuilderExtensionsWithAppAndOptionsNullAndPort()
        {
            FluentActions.Invoking(() =>
                HealthCheckApplicationBuilderExtensions.UseHealthChecks(null, null, new PathString(""), 25, null)
            )
                .Should()
                .Throw<ArgumentNullException>();
        }

        #region [ Arrange ]

        public class StartupWithhHealthCheckOptions : Startup
        {
            public StartupWithhHealthCheckOptions() : base() { }

            public override void AddServices(IServiceCollection services)
            {
                services.AddHealthChecks()
                    .AddCheck("Healthy", () => HealthCheckResult.Healthy(), new[] { "TagPredicate01" })
                    .AddCheck("Healthy2", () => HealthCheckResult.Unhealthy(), new[] { "TagPredicate02" });
            }

            public override void SetupUseHealthChecks(IAppBuilder app, IServiceProvider servicesProvider)
            {
                app
                    .UseHealthChecks(
                        servicesProvider,
                        new PathString("/hc-tag-predicate1"),
                        new Microsoft.AspNet.Diagnostics.HealthChecks.HealthCheckOptions
                        {
                            Predicate = _ => _.Tags.Contains("TagPredicate01"),
                            AllowCachingResponses = true,
                            ResponseWriter = (context, result) =>
                            {
                                context.Response.ContentType = "text/plain";
                                return context.Response.WriteAsync(result.Status.ToString());
                            }
                        }
                    )
                    .UseHealthChecks(
                        servicesProvider,
                        new PathString("/hc-tag-predicate2"),
                        new Microsoft.AspNet.Diagnostics.HealthChecks.HealthCheckOptions
                        {
                            Predicate = _ => _.Tags.Contains("TagPredicate02"),
                            AllowCachingResponses = true,
                            ResponseWriter = (context, result) =>
                            {
                                context.Response.ContentType = "text/plain";
                                return context.Response.WriteAsync(result.Status.ToString());
                            }
                        }
                    );
            }
        }

        public class StartupWithPort : Startup
        {
            public StartupWithPort() : base() { }

            public override void AddServices(IServiceCollection services)
            {
                services.AddHealthChecks()
                    .AddCheck("Healthy", () => HealthCheckResult.Healthy());
            }

            public override void SetupUseHealthChecks(IAppBuilder app, IServiceProvider servicesProvider)
            {
                app
                    .UseHealthChecks(
                        servicesProvider,
                        new PathString("/hc"),
                        12345
                    );
            }
        }

        public class HealthChecksTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new AppBuilder<StartupHealthy>($"http://localhost:{Port.Next()}"), "Healthy" };
                yield return new object[] { new AppBuilder<StartupUnhealthy>($"http://localhost:{Port.Next()}"), "Unhealthy" };
                yield return new object[] { new AppBuilder<StartupDegraded>($"http://localhost:{Port.Next()}"), "Degraded" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public class StartupHealthy : Startup
            {
                public StartupHealthy() : base() { }

                public override void AddServices(IServiceCollection services) =>
                    services
                        .AddHealthChecks()
                        .AddCheck("Healthy", () => HealthCheckResult.Healthy());
            }

            public class StartupUnhealthy : Startup
            {
                public StartupUnhealthy() : base() { }

                public override void AddServices(IServiceCollection services) =>
                    services
                        .AddHealthChecks()
                        .AddCheck("Unhealthy", () => HealthCheckResult.Unhealthy());
            }

            public class StartupDegraded : Startup
            {
                public StartupDegraded() : base() { }

                public override void AddServices(IServiceCollection services) =>
                    services
                        .AddHealthChecks()
                        .AddCheck("Degraded", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded());
            }
        }
        #endregion [ Arrange ]
    }
}
