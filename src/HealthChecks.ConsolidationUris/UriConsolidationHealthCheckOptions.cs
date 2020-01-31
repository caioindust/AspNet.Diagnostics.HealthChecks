using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HealthChecks.UrisConsolidation
{
    public interface IUrisConsolidationOptions
    {
        IUrisConsolidationOptions UseGet();

        IUrisConsolidationOptions UsePost();

        IUrisConsolidationOptions UseHttpMethod(HttpMethod methodToUse);

        IUrisConsolidationOptions UseTimeout(TimeSpan timeout);

        IUrisConsolidationOptions ExpectHttpCode(int codeToExpect);

        IUrisConsolidationOptions ExpectHttpCodes(int minCodeToExpect, int maxCodeToExpect);

        IUrisConsolidationOptions AddCustomHeader(string name, string value);
    }

    public class UrisConsolidationOptions : IUrisConsolidationOptions
    {
        public HttpMethod HttpMethod { get; private set; }

        public TimeSpan Timeout { get; private set; }

        public (int Min, int Max)? ExpectedHttpCodes { get; private set; }

        public Uri Uri { get; }

        private readonly List<(string Name, string Value)> _headers = new List<(string Name, string Value)>();

        internal IEnumerable<(string Name, string Value)> Headers => _headers;

        public UrisConsolidationOptions(Uri uri)
        {
            Uri = uri;
            ExpectedHttpCodes = null;
            HttpMethod = null;
        }

        public IUrisConsolidationOptions AddCustomHeader(string name, string value)
        {
            _headers.Add((name, value));
            return this;
        }

        IUrisConsolidationOptions IUrisConsolidationOptions.UseGet()
        {
            HttpMethod = HttpMethod.Get;
            return this;
        }

        IUrisConsolidationOptions IUrisConsolidationOptions.UsePost()
        {
            HttpMethod = HttpMethod.Post;
            return this;
        }

        IUrisConsolidationOptions IUrisConsolidationOptions.ExpectHttpCode(int codeToExpect)
        {
            ExpectedHttpCodes = (codeToExpect, codeToExpect);
            return this;
        }

        IUrisConsolidationOptions IUrisConsolidationOptions.ExpectHttpCodes(int minCodeToExpect, int maxCodeToExpect)
        {
            ExpectedHttpCodes = (minCodeToExpect, maxCodeToExpect);
            return this;
        }

        IUrisConsolidationOptions IUrisConsolidationOptions.UseHttpMethod(HttpMethod methodToUse)
        {
            HttpMethod = methodToUse;
            return this;
        }

        IUrisConsolidationOptions IUrisConsolidationOptions.UseTimeout(TimeSpan timeout)
        {
            Timeout = timeout;
            return this;
        }
    }

    public class UrisConsolidationHealthCheckOptions
    {
        private readonly List<UrisConsolidationOptions> _urisOptions = new List<UrisConsolidationOptions>();

        internal IEnumerable<UrisConsolidationOptions> UrisOptions => _urisOptions;

        internal HttpMethod HttpMethod { get; private set; }

        internal TimeSpan Timeout { get; private set; }

        internal (int Min, int Max) ExpectedHttpCodes { get; private set; }

        public UrisConsolidationHealthCheckOptions()
        {
            const int Min_Status200OK_Range = 200;
            const int Max_Status200OK_Range = 299;
            ExpectedHttpCodes = (Min_Status200OK_Range, Max_Status200OK_Range);              // DEFAULT  = HTTP Succesful status codes
            HttpMethod = HttpMethod.Get;
            Timeout = TimeSpan.FromSeconds(10);
        }

        public UrisConsolidationHealthCheckOptions UseGet()
        {
            HttpMethod = HttpMethod.Get;
            return this;
        }

        public UrisConsolidationHealthCheckOptions UsePost()
        {
            HttpMethod = HttpMethod.Post;
            return this;
        }

        public UrisConsolidationHealthCheckOptions UseHttpMethod(HttpMethod methodToUse)
        {
            HttpMethod = methodToUse;
            return this;
        }

        public UrisConsolidationHealthCheckOptions UseTimeout(TimeSpan timeout)
        {
            Timeout = timeout;
            return this;
        }

        public UrisConsolidationHealthCheckOptions AddUri(Uri uriToAdd, Action<IUrisConsolidationOptions> setup = null)
        {
            var uri = new UrisConsolidationOptions(uriToAdd);
            setup?.Invoke(uri);

            _urisOptions.Add(uri);

            return this;
        }

        public UrisConsolidationHealthCheckOptions ExpectHttpCode(int codeToExpect)
        {
            ExpectedHttpCodes = (codeToExpect, codeToExpect);
            return this;
        }

        public UrisConsolidationHealthCheckOptions ExpectHttpCodes(int minCodeToExpect, int maxCodeToExpect)
        {
            ExpectedHttpCodes = (minCodeToExpect, maxCodeToExpect);
            return this;
        }

        internal static UrisConsolidationHealthCheckOptions CreateFromUris(IEnumerable<Uri> uris)
        {
            var options = new UrisConsolidationHealthCheckOptions();

            foreach (var item in uris)
            {
                options.AddUri(item);
            }

            return options;
        }
    }
}
