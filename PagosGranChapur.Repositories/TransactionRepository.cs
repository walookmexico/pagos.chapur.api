using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;

namespace PagosGranChapur.Repositories
{
    public interface ITransactionRepository: IRepository<Transaction> { }

    public class TransactionRepository: RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
