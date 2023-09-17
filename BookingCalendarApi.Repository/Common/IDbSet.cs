using System.Linq;

namespace BookingCalendarApi.Repository.Common
{
    public interface IDbSet<TEntity> : IQueryable<TEntity>
        where TEntity : class
    {
    }
}
