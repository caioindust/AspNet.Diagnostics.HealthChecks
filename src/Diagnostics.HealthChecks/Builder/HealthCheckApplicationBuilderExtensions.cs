// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Owin;
using System;

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension methods for the <see cref="OwinHealthCheckMiddleware"/>.
    /// </summary>
    public static class HealthCheckApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests. If <paramref name="path"/> is set to a non-empty
        /// value, the health check middleware will process requests with a URL that matches the provided value
        /// of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') character.
        /// </para>
        /// <para>
        /// The health check middleware will use default settings from <see cref="IOptions{HealthCheckOptions}"/>.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, IServiceProvider serviceProvider, PathString path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            UseHealthChecksOwin(app, serviceProvider, path, port: null, default);
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests. If <paramref name="path"/> is set to a non-empty
        /// value, the health check middleware will process requests with a URL that matches the provided value
        /// of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') character.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, IServiceProvider serviceProvider, PathString path, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecksOwin(app, serviceProvider, path, port: null, options);
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="port">The port to listen on. Must be a local port on which the server is listening.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests on the specified port. If <paramref name="path"/> is
        /// set to a non-empty value, the health check middleware will process requests with a URL that matches the
        /// provided value of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/')
        /// character.
        /// </para>
        /// <para>
        /// The health check middleware will use default settings from <see cref="IOptions{HealthCheckOptions}"/>.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, IServiceProvider serviceProvider, PathString path, int port)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            UseHealthChecksOwin(app, serviceProvider, path, port, default);
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="port">The port to listen on. Must be a local port on which the server is listening.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests on the specified port. If <paramref name="path"/> is
        /// set to a non-empty value, the health check middleware will process requests with a URL that matches the
        /// provided value of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/')
        /// character.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, IServiceProvider serviceProvider, PathString path, int port, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecksOwin(app, serviceProvider, path, port, options);
            return app;
        }

        private static void UseHealthChecksOwin(IAppBuilder app, IServiceProvider serviceProvider, PathString path, int? port, HealthCheckOptions healthCheckOptions)
        {
            // NOTE: we explicitly don't use Map here because it's really common for multiple health
            // check middleware to overlap in paths. Ex: `/health`, `/health/detailed` - this is order
            // sensititive with Map, and it's really surprising to people.
            //
            // See:
            // https://github.com/aspnet/Diagnostics/issues/511
            // https://github.com/aspnet/Diagnostics/issues/512
            // https://github.com/aspnet/Diagnostics/issues/514

            bool predicate(IOwinContext ctx)
            {
                return

                    // Process the port if we have one
                    (port == null || ctx.Request.LocalPort == port) &&

                    // We allow you to listen on all URLs by providing the empty PathString.
                    (!path.HasValue ||

                        // If you do provide a PathString, want to handle all of the special cases that
                        // StartsWithSegments handles, but we also want it to have exact match semantics.
                        //
                        // Ex: /Foo/ == /Foo (true)
                        // Ex: /Foo/Bar == /Foo (false)
                        (ctx.Request.Path.StartsWithSegments(path, out var remaining) && string.IsNullOrEmpty(remaining.Value)));
            }

            var healthCheckService = serviceProvider.GetService<HealthCheckService>();

            app.MapWhen(predicate, b => b.Use<OwinHealthCheckMiddleware>(Options.Create(healthCheckOptions ?? new HealthCheckOptions()), healthCheckService));
        }
    }
}
