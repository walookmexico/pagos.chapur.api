using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Repositories
{
    public interface ILogTransactionRepository : IRepository<LogTransaction>
    {
    }

    public class LogTransactionRepository: RepositoryBase<LogTransaction> , ILogTransactionRepository
    {
        public LogTransactionRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
