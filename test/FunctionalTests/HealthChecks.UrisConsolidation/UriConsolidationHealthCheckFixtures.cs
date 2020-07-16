using Common.Tests;
using System;

namespace Diagnostics.HealthChecks
{
    public class UriConsolidationHealthCheckFixtures : IDisposable
    {
        private readonly AppBuilder<Common.Tests.Api.StartupApi> _app1;
        private readonly AppBuilder<Common.Tests.Api.StartupApi> _app2;
        private readonly AppBuilder<Common.Tests.Api.StartupApi> _app3;        

        public UriConsolidationHealthCheckFixtures()
        {
            _app1 = new AppBuilder<Common.Tests.Api.StartupApi>("http://localhost:12001");
            _app2 = new AppBuilder<Common.Tests.Api.StartupApi>("http://localhost:12002");
            _app3 = new AppBuilder<Common.Tests.Api.StartupApi>("http://localhost:12003");                    
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                _app1.Dispose();
                _app2.Dispose();
                _app3.Dispose();                
            }

            disposedValue = true;

        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
