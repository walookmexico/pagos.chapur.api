using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;

namespace PagosGranChapur.Repositories
{
    public interface IPlatformRepository : IRepository<Platform> { }

    public class PlatformRepository : RepositoryBase<Platform>, IPlatformRepository
    {
        public PlatformRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
