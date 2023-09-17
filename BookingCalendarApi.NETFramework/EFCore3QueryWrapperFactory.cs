using BookingCalendarApi.Repository.Common;
using System.Linq;

namespace BookingCalendarApi.Repository.NETFramework
{
    public class EFCore3QueryWrapperFactory : IQueryWrapperFactory
    {
        public IQueryWrapper<TEntity> Create<TEntity>(IQueryable<TEntity> query) where TEntity : class
        {
            return new EFCore3QueryWrapper<TEntity>(query);
        }
    }
}
