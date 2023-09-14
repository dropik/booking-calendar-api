using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace BookingCalendarApi.Repository.NETCore
{
    public class DbSetWrapper<TEntity> : IDbSet<TEntity>, IAsyncEnumerable<TEntity>
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
            return _entities.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
        }
    }
}
