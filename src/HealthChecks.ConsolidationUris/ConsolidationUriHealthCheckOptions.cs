using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HealthChecks.ConsolidationUris
{
    public interface IConsolidationUriOptions
    {
        IConsolidationUriOptions UseGet();
        IConsolidationUriOptions UsePost();
        IConsolidationUriOptions UseHttpMethod(HttpMethod methodToUse);
        IConsolidationUriOptions ExpectHttpCode(int codeToExpect);
        IConsolidationUriOptions ExpectHttpCodes(int minCodeToExpect, int maxCodeToExpect);
        IConsolidationUriOptions AddCustomHeader(string name, string value);
    }

    public class ConsolidationUriOptions : IConsolidationUriOptions
    {
        public HttpMethod HttpMethod { get; private set; }

        public (int Min, int Max)? ExpectedHttpCodes { get; private set; }

        public Uri Uri { get; }

        private readonly List<(string Name, string Value)> _headers = new List<(string Name, string Value)>();

        internal IEnumerable<(string Name, string Value)> Headers => _headers;

        public ConsolidationUriOptions(Uri uri)
        {
            Uri = uri;
            ExpectedHttpCodes = null;
            HttpMethod = null;
        }

        public IConsolidationUriOptions AddCustomHeader(string name, string value)
        {
            _headers.Add((name, value));
            return this;
        }

        IConsolidationUriOptions IConsolidationUriOptions.UseGet()
        {
            HttpMethod = HttpMethod.Get;
            return this;
        }

        IConsolidationUriOptions IConsolidationUriOptions.UsePost()
        {
            HttpMethod = HttpMethod.Post;
            return this;
        }

        IConsolidationUriOptions IConsolidationUriOptions.ExpectHttpCode(int codeToExpect)
        {
            ExpectedHttpCodes = (codeToExpect, codeToExpect);
            return this;
        }

        IConsolidationUriOptions IConsolidationUriOptions.ExpectHttpCodes(int minCodeToExpect, int maxCodeToExpect)
        {
            ExpectedHttpCodes = (minCodeToExpect, maxCodeToExpect);
            return this;
        }

        IConsolidationUriOptions IConsolidationUriOptions.UseHttpMethod(HttpMethod methodToUse)
        {
            HttpMethod = methodToUse;
            return this;
        }
    }

    public class ConsolidationUriHealthCheckOptions
    {
        private readonly List<ConsolidationUriOptions> _urisOptions = new List<ConsolidationUriOptions>();

        internal IEnumerable<ConsolidationUriOptions> UrisOptions => _urisOptions;

        internal HttpMethod HttpMethod { get; private set; }

        internal (int Min, int Max) ExpectedHttpCodes { get; private set; }        

        public ConsolidationUriHealthCheckOptions()
        {
            ExpectedHttpCodes = (200, 299);              // DEFAULT  = HTTP Succesful status codes
            HttpMethod = HttpMethod.Get;
        }

        public ConsolidationUriHealthCheckOptions UseGet()
        {
            HttpMethod = HttpMethod.Get;
            return this;
        }

        public ConsolidationUriHealthCheckOptions UsePost()
        {
            HttpMethod = HttpMethod.Post;
            return this;
        }

        public ConsolidationUriHealthCheckOptions UseHttpMethod(HttpMethod methodToUse)
        {
            HttpMethod = methodToUse;
            return this;
        }

        public ConsolidationUriHealthCheckOptions AddUri(Uri uriToAdd, Action<IConsolidationUriOptions> setup = null)
        {
            var uri = new ConsolidationUriOptions(uriToAdd);
            setup?.Invoke(uri);

            _urisOptions.Add(uri);

            return this;
        }

        public ConsolidationUriHealthCheckOptions ExpectHttpCode(int codeToExpect)
        {
            ExpectedHttpCodes = (codeToExpect, codeToExpect);
            return this;
        }

        public ConsolidationUriHealthCheckOptions ExpectHttpCodes(int minCodeToExpect, int maxCodeToExpect)
        {
            ExpectedHttpCodes = (minCodeToExpect, maxCodeToExpect);
            return this;
        }

        internal static ConsolidationUriHealthCheckOptions CreateFromUris(IEnumerable<Uri> uris)
        {
            var options = new ConsolidationUriHealthCheckOptions();

            foreach (var item in uris)
            {
                options.AddUri(item);
            }

            return options;
        }
    }
}
