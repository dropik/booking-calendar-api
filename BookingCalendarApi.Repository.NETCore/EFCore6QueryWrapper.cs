﻿using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingCalendarApi.Repository.NETCore
{
    public class EFCore6QueryWrapper<TEntity> : IQueryWrapper<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _query;

        public EFCore6QueryWrapper(IQueryable<TEntity> query)
        {
            _query = query;
        }

        public async Task<bool> AnyAsync()
        {
            return await _query.AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _query.AnyAsync(predicate);
        }

        public async Task<TEntity> SingleAsync()
        {
            return await _query.SingleAsync();
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _query.SingleAsync(predicate);
        }

        public async Task<TEntity?> SingleOrDefaultAsync()
        {
            return await _query.SingleOrDefaultAsync();
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _query.SingleOrDefaultAsync(predicate);
        }

        public async Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector) where TKey : notnull
        {
            return await _query.ToDictionaryAsync(keySelector);
        }

        public Task<List<TEntity>> ToListAsync()
        {
            return _query.ToListAsync();
        }

        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation)
        {
            return _query.Include(navigation);
        }
    }
}
