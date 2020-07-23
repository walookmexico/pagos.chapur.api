using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;

namespace PagosGranChapur.Repositories
{
    public interface IUserRepository : IRepository<User> { }

    public class UserRepository: RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory)
           : base(dbFactory) { }
    }
}
