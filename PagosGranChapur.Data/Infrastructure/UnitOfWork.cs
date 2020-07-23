using System;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbFactory _dbFactory;
        private PagosGranChapurContext _dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this._dbFactory = dbFactory;
        }

        public PagosGranChapurContext DbContext
        {
            get
            {
                return _dbContext ?? (_dbContext = _dbFactory.Init());
            }
        }

        public bool Commit()
        {
            DbContext.Commit();
            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                try
                {
                    if (DbContext != null && DbContext.Database.Connection.State == System.Data.ConnectionState.Open)
                        DbContext.Database.Connection.Close();
                }
                catch (ObjectDisposedException)
                {
                    // do nothing, the objectContext has already been disposed
                }

                if (DbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }
            
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
