using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace BookingCalendarApi.Repository.NETFramework
{
    public class DbSetWrapper<TEntity> : DbSet<TEntity>, Common.IDbSet<TEntity>, IAsyncEnumerable<TEntity>
        where TEntity : class
    {
        private readonly DbSet<TEntity> _entities;

        public DbSetWrapper(DbSet<TEntity> entities)
        {
            _entities = entities;
        }

        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> fieldSelector)
            where TProperty : class
            => _entities.Include(fieldSelector);

        public Type ElementType => _entities.AsQueryable().ElementType;

        public Expression Expression => _entities.AsQueryable().Expression;

        public IQueryProvider Provider => _entities.AsQueryable().Provider;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _entities.AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.AsQueryable().GetEnumerator();
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _entities.AsAsyncEnumerable().GetAsyncEnumerator();
        }
    }
}
