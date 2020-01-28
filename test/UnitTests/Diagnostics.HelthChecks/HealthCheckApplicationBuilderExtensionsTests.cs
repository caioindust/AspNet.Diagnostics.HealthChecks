using FluentAssertions;
using Microsoft.AspNet.Builder;
using Microsoft.Owin;
using System;
using Xunit;

namespace Diagnostics.HelthChecks
{
    public class HealthCheckApplicationBuilderExtensionsTests
    {
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
    }
}
