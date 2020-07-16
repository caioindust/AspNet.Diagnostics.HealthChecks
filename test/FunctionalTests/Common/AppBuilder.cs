using Common.Tests;
using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using System.Threading;

namespace Diagnostics.HealthChecks
{
    public class AppBuilder<T> : IDisposable
    {
        public string Url { get; }

        private readonly IDisposable _webContext;

        public AppBuilder(string url)
        {
            Url = url;
            _webContext = WebApp.Start<T>(Url);        
        }
      
        #region IDisposable Support

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
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
