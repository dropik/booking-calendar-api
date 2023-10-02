using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookingCalendarApi.Repository.Common
{
    public interface IQueryWrapper<TEntity> where TEntity : class
    {
        Task<List<TEntity>> ToListAsync();
        Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector);
        Task<TEntity> SingleAsync();
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultAsync();
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync();
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation);
    }
}
