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
    public interface IStoreRepository : IRepository<Store> { }

    public class StoreRepository: RepositoryBase<Store>, IStoreRepository
    {
        public StoreRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
