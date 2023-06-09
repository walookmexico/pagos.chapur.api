﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Infrastructure
{
    public abstract class RepositoryBase<T> where T : class
    {
        #region Properties

        private PagosGranChapurContext dataContext;
        private readonly IDbSet<T> dbSet;

        protected IDbFactory DbFactory { get; private set; }

        protected PagosGranChapurContext DbContext
        {
            get { return dataContext ?? (dataContext = DbFactory.Init()); }
        }

        #endregion

        protected RepositoryBase(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            dbSet = DbContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
        }

        public virtual T GetById(int id)
        {
            return dbSet.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault<T>();
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await dbSet.Where(match).ToListAsync();
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await dbSet.SingleOrDefaultAsync(match);
        }

        public virtual async Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T t)
        {
            var addAsyncTask = new TaskCompletionSource<T>();
            dbSet.Add(t);
            addAsyncTask.SetResult(t);
            await addAsyncTask.Task;
            
            return t;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var deleteAsyncTask = new TaskCompletionSource<T>();
            dbSet.Remove(entity);
            deleteAsyncTask.SetResult(entity);
            await deleteAsyncTask.Task;
        }
    }
}
