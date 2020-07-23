using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;

namespace PagosGranChapur.Repositories
{
    public interface IStoreRepository : IRepository<Store> { }

    public class StoreRepository: RepositoryBase<Store>, IStoreRepository
    {
        public StoreRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
