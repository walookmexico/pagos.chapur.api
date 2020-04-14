using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Repositories
{
    public interface IUserRepository : IRepository<User> { }

    public class UserRepository: RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory)
           : base(dbFactory) { }
    }
}
