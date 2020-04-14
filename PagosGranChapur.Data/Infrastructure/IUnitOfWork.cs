using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        bool Commit();
        Task<int> SaveChangesAsync();

    }
}
