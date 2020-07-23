using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;

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
