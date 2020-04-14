using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Repositories
{
    public interface ITransactionRepository: IRepository<Transaction> { }

    public class TransactionRepository: RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
