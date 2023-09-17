using BookingCalendarApi.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookingCalendarApi.Repository.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<List<TEntity>> ToListAsync<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).ToListAsync();
        }

        public static async Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey, TEntity>(this IQueryable<TEntity> query, Func<TEntity, TKey> keySelector) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).ToDictionaryAsync(keySelector);
        }
        
        public static async Task<TEntity> SingleAsync<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).SingleAsync();
        }

        public static async Task<TEntity> SingleAsync<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).SingleAsync(predicate);
        }

        public static async Task<TEntity> SingleOrDefaultAsync<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).SingleOrDefaultAsync();
        }

        public static async Task<TEntity> SingleOrDefaultAsync<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).SingleOrDefaultAsync(predicate);
        }

        public static async Task<bool> AnyAsync<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).AnyAsync();
        }

        public static async Task<bool> AnyAsync<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return await QueryWrapperFactory.Current.Create(query).AnyAsync(predicate);
        }

        public static IQueryable<TEntity> Include<TEntity, TProperty>(this IQueryable<TEntity> query, Expression<Func<TEntity, TProperty>> navigation) where TEntity : class
        {
            return QueryWrapperFactory.Current.Create(query).Include(navigation);
        }
    }
}
