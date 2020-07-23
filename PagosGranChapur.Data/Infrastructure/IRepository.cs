using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task<T> AddAsync(T t);
        void Update(T entity);

        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        Task DeleteAsync(T entity);
      
        T GetById(int id);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        Task<ICollection<T>> GetAllAsync();
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);

   
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate);

    }
}
