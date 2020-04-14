using System;
using System.Collections.Generic;
using System.Text;

namespace PagosGranChapur.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        PagosGranChapurContext _dbContext;

        public PagosGranChapurContext Init()
        {
            return _dbContext ?? (_dbContext = new PagosGranChapurContext());
        }

        protected override void DisposeCore()
        {
            _dbContext?.Dispose();
        }
    }
}
