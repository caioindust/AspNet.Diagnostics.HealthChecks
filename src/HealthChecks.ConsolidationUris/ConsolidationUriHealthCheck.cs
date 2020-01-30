using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.ConsolidationUris
{
    public class ConsolidationUriHealthCheck : IHealthCheck
    {
        private readonly ConsolidationUriHealthCheckOptions _options;
        private readonly Func<IEnumerable<HealthCheckResult>, HealthCheckResult> _consolidation;

        public ConsolidationUriHealthCheck(
            ConsolidationUriHealthCheckOptions options,
            Func<IEnumerable<HealthCheckResult>, HealthCheckResult> consolidation
        )
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _consolidation = consolidation ?? throw new ArgumentNullException(nameof(consolidation));
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var defaultHttpMethod = _options.HttpMethod;
            var defaultCodes = _options.ExpectedHttpCodes;
            var idx = 0;

            try
            {
                var healthCheckResults = new List<HealthCheckResult>();

                foreach (var item in _options.UrisOptions)
                {
                    var method = item.HttpMethod ?? defaultHttpMethod;
                    var expectedCodes = item.ExpectedHttpCodes ?? defaultCodes;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return new HealthCheckResult(context.Registration.FailureStatus, description: $"{nameof(ConsolidationUriHealthCheck)} execution is cancelled.");
                    }

                    using (var httpClient = new HttpClient())
                    {
                        var requestMessage = new HttpRequestMessage(method, item.Uri);

                        foreach (var header in item.Headers)
                        {
                            requestMessage.Headers.Add(header.Name, header.Value);
                        }

                        var response = await httpClient.SendAsync(requestMessage);

                        if (!((int)response.StatusCode >= expectedCodes.Min && (int)response.StatusCode <= expectedCodes.Max))
                        {
                            healthCheckResults.Add(
                                new HealthCheckResult(
                                    context.Registration.FailureStatus,
                                    description: $"Discover endpoint #{idx} is not responding with code in {expectedCodes.Min}...{expectedCodes.Max} range, the current status is {response.StatusCode}."
                                 )
                            );
                        }

                        ++idx;
                    }
                }

                return _consolidation(healthCheckResults);
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }

}
