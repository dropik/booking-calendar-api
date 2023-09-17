using BookingCalendarApi.Repository.Common;

namespace BookingCalendarApi.Repository.NETCore
{
    public class EFCore6QueryWrapperFactory : IQueryWrapperFactory
    {
        public IQueryWrapper<TEntity> Create<TEntity>(IQueryable<TEntity> query) where TEntity : class
        {
            return new EFCore6QueryWrapper<TEntity>(query);
        }
    }
}
