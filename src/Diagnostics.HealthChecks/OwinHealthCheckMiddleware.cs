// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Diagnostics.HealthChecks
{
    public class OwinHealthCheckMiddleware : OwinMiddleware
    {
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly HealthCheckService _healthCheckService;

        public OwinHealthCheckMiddleware(OwinMiddleware next,
            IOptions<HealthCheckOptions> healthCheckOptions,
            HealthCheckService healthCheckService
        ) : base(next)
        {
            if (healthCheckOptions == null)
            {
                throw new ArgumentNullException(nameof(healthCheckOptions));
            }

            _healthCheckOptions = healthCheckOptions.Value;
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        }

        ///// <summary>
        ///// Processes a request.
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns></returns>
        public override Task Invoke(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return InvokeInternal(context);
        }

        private async Task InvokeInternal(IOwinContext context)
        {
            // Get results
            var result = await _healthCheckService.CheckHealthAsync(_healthCheckOptions?.Predicate, context.Request.CallCancelled);

            // Map status to response code - this is customizable via options.
            if (!_healthCheckOptions.ResultStatusCodes.TryGetValue(result.Status, out var statusCode))
            {
                var message =
                    $"No status code mapping found for {nameof(HealthStatus)} value: {result.Status}." +
                    $"{nameof(HealthCheckOptions)}.{nameof(HealthCheckOptions.ResultStatusCodes)} must contain" +
                    $"an entry for {result.Status}.";

                throw new InvalidOperationException(message);
            }

            context.Response.StatusCode = statusCode;

            if (!_healthCheckOptions.AllowCachingResponses)
            {
                // Similar to: https://github.com/aspnet/Security/blob/7b6c9cf0eeb149f2142dedd55a17430e7831ea99/src/Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationHandler.cs#L377-L379
                var headers = context.Response.Headers;
                headers[HeaderNames.CacheControl] = "no-store, no-cache";
                headers[HeaderNames.Pragma] = "no-cache";
                headers[HeaderNames.Expires] = "Thu, 01 Jan 1970 00:00:00 GMT";
            }

            if (_healthCheckOptions.ResponseWriter != null)
            {
                await _healthCheckOptions.ResponseWriter(context, result);
            }
        }
    }
}
