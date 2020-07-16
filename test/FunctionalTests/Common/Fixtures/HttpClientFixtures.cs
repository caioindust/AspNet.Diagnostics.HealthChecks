using System;
using System.Net.Http;
using System.Threading;

namespace Common.Tests
{
    public class HttpClientFixtures: IDisposable
    {
        private readonly HttpClient _client;

        public HttpClientFixtures()
        {
            _client = new HttpClient();
        }

        public HttpResponseMessage Get(string requestUri, CancellationToken cancellationToken = default) =>
            _client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

        public HttpResponseMessage Post(string requestUri, HttpContent httpContent, CancellationToken cancellationToken = default) =>
            _client.PostAsync(requestUri, httpContent, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

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
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HttpClientFixtures()
        {
            Dispose(false);
        }

        #endregion IDisposable Support
    }
}
