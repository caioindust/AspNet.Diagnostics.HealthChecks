using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

namespace Diagnostics.HealthChecks
{
    public class AppBuilder<T> : IDisposable
    {
        public string Url { get; }

        private readonly IDisposable _webContext;
        private readonly HttpClient _client;

        public AppBuilder(string url)
        {
            Url = url;
            _webContext = WebApp.Start<T>(Url);
            _client = new HttpClient();
        }

        public HttpResponseMessage Get(string requestUri) =>
            _client.GetAsync(requestUri).ConfigureAwait(false).GetAwaiter().GetResult();

        public string GetContent(string requestUri) =>
             GetContent(Get(requestUri));

        public string GetContent(HttpResponseMessage response) =>
             response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        #region IDisposable Support

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _client.Dispose();
                _webContext.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AppBuilder()
        {
            Dispose(false);
        }

        #endregion IDisposable Support
    }
}
