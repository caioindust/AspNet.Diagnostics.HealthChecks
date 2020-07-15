using Common.Tests;
using System;

namespace Diagnostics.HealthChecks
{
    public class UriConsolidationHealthCheckFixtures : IDisposable
    {
        private readonly AppBuilder<Common.Tests.BasicArranges.StartupHealthy> _app1;
        private readonly AppBuilder<Common.Tests.BasicArranges.StartupHealthy> _app2;
        private readonly AppBuilder<Common.Tests.BasicArranges.StartupHealthy> _app3;
        private readonly AppBuilder<Common.Tests.BasicArranges.StartupUnhealthy> _app4;

        public UriConsolidationHealthCheckFixtures()
        {
            _app1 = new AppBuilder<Common.Tests.BasicArranges.StartupHealthy>($"http://localhost:12001");
            _app2 = new AppBuilder<Common.Tests.BasicArranges.StartupHealthy>($"http://localhost:12002");
            _app3 = new AppBuilder<Common.Tests.BasicArranges.StartupHealthy>($"http://localhost:12003");
            _app4 = new AppBuilder<Common.Tests.BasicArranges.StartupUnhealthy>($"http://localhost:12004");
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
                _app4.Dispose();
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
