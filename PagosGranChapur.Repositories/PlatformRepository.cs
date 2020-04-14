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
    public interface IPlatformRepository : IRepository<Platform> { }

    public class PlatformRepository : RepositoryBase<Platform>, IPlatformRepository
    {
        public PlatformRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
