using System;

namespace PagosGranChapur.Data.Infrastructure
{
    public class Disposable : IDisposable
    {
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();

            }

            isDisposed = true;
        }

        protected virtual void DisposeCore()
        {
        }

        ~Disposable()
        {
            Dispose(false);
        }

    }
}
