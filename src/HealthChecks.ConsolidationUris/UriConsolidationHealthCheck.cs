using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.UrisConsolidation
{
    public class UrisConsolidationHealthCheck : IHealthCheck
    {
        private readonly UrisConsolidationHealthCheckOptions _options;
        private readonly Func<IEnumerable<HealthCheckResult>, HealthCheckResult> _consolidation;

        public UrisConsolidationHealthCheck(
            UrisConsolidationHealthCheckOptions options,
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
            var defaultTimeout = _options.Timeout;
            var idx = 0;

            var healthCheckResults = new List<HealthCheckResult>();

            foreach (var item in _options.UrisOptions)
            {
                var method = item.HttpMethod ?? defaultHttpMethod;
                var expectedCodes = item.ExpectedHttpCodes ?? defaultCodes;
                var timeout = item.Timeout != TimeSpan.Zero ? item.Timeout : defaultTimeout;

                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return new HealthCheckResult(context.Registration.FailureStatus, description: $"{nameof(UrisConsolidationHealthCheck)} execution is cancelled.");
                    }

                    using (var httpClient = new HttpClient())
                    {
                        var requestMessage = new HttpRequestMessage(method, item.Uri);

                        foreach (var header in item.Headers)
                        {
                            requestMessage.Headers.Add(header.Name, header.Value);
                        }

                        using (var timeoutSource = new CancellationTokenSource(timeout))
                        using (var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken))
                        {
                            var response = await httpClient.SendAsync(requestMessage, linkedSource.Token);

                            if (!((int)response.StatusCode >= expectedCodes.Min && (int)response.StatusCode <= expectedCodes.Max))
                            {
                                healthCheckResults.Add(
                                    new HealthCheckResult(
                                        context.Registration.FailureStatus,
                                        description: $"Discover endpoint #{idx} [{item.Uri}] is not responding with code in {expectedCodes.Min}...{expectedCodes.Max} range, the current status is {response.StatusCode}."
                                     )
                                );
                            }
                            else
                            {
                                healthCheckResults.Add(HealthCheckResult.Healthy());
                            }

                            ++idx;
                        }
                    }
                }
                catch (Exception ex)
                {
                    healthCheckResults.Add(
                        new HealthCheckResult(
                            context.Registration.FailureStatus,
                            exception: ex,
                            description: $"Discover endpoint #{idx} [{item.Uri}] is not responding."
                        )
                    );
                }
            }

            try
            {
                return _consolidation(healthCheckResults);
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
