using System;
using System.Linq;
using System.Linq.Expressions;

namespace BookingCalendarApi.Repository.Common
{
    public interface IDbSet<TEntity> : IQueryable<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> fieldSelector)
            where TProperty : class;
    }
}
