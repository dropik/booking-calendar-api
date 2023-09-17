using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace BookingCalendarApi.Repository
{
    public class DbSetWrapper<TEntity> : IDbSet<TEntity>, IAsyncEnumerable<TEntity>
        where TEntity : class
    {
        private readonly DbSet<TEntity> _entities;
        private readonly long _currentStructureId;

        public DbSetWrapper(DbSet<TEntity> entities, long currentStructureId)
        {
            _entities = entities;
            _currentStructureId = currentStructureId;
        }

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

        //private IQueryable<TEntity> GetQueryable()
        //{
        //    var query = _entities.AsQueryable();
        //    if (typeof(IStructureData).IsAssignableFrom(typeof(TEntity)))
        //    {
        //        query = query.Where(e => ((IStructureData)e).StructureId == _currentStructureId).AsQueryable();
        //    }
        //    return query;
        //}
    }
}
