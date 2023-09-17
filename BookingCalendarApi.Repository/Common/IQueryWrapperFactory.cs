using System.Linq;

namespace BookingCalendarApi.Repository.Common
{
    public interface IQueryWrapperFactory
    {
        IQueryWrapper<TEntity> Create<TEntity>(IQueryable<TEntity> query) where TEntity : class;
    }
}
