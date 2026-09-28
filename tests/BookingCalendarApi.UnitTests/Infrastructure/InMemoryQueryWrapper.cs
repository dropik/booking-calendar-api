using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using BookingCalendarApi.Repository.Common;

namespace BookingCalendarApi.UnitTests.Infrastructure
{
    /// <summary>
    /// LINQ-to-objects implementation of the repository query wrapper, so that services using
    /// <c>RepositoryExtensions</c> can run against <see cref="InMemoryRepository"/>.
    /// </summary>
    public class InMemoryQueryWrapper<TEntity> : IQueryWrapper<TEntity> where TEntity : class
    {
        private readonly IQueryable<TEntity> _query;

        public InMemoryQueryWrapper(IQueryable<TEntity> query)
        {
            _query = query;
        }

        public Task<List<TEntity>> ToListAsync() => Task.FromResult(_query.ToList());

        public Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector) where TKey : notnull
            => Task.FromResult(_query.ToDictionary(keySelector));

        public Task<TEntity> SingleAsync() => Task.FromResult(_query.Single());

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate) => Task.FromResult(_query.Single(predicate));

        public Task<TEntity?> SingleOrDefaultAsync() => Task.FromResult(_query.SingleOrDefault());

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) => Task.FromResult(_query.SingleOrDefault(predicate));

        public Task<bool> AnyAsync() => Task.FromResult(_query.Any());

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) => Task.FromResult(_query.Any(predicate));

        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation) => _query;
    }

    public class InMemoryQueryWrapperFactory : IQueryWrapperFactory
    {
        public IQueryWrapper<TEntity> Create<TEntity>(IQueryable<TEntity> query) where TEntity : class
            => new InMemoryQueryWrapper<TEntity>(query);
    }

    internal static class QueryWrapperSetup
    {
        // QueryWrapperFactory.Current is a process wide static used by RepositoryExtensions;
        // every unit test uses the same in-memory implementation, so setting it once is safe
        [ModuleInitializer]
        internal static void Initialize()
        {
            QueryWrapperFactory.Current = new InMemoryQueryWrapperFactory();
        }
    }
}
