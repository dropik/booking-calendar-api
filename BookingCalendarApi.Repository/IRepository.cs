using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Repository
{
    public interface IRepository
    {
        IQueryable<Structure> Structures { get; }
        IQueryable<User> Users { get; }
        IQueryable<UserRefreshToken> UserRefreshTokens { get; }
        IQueryable<Nation> Nations { get; }
        IQueryable<Floor> Floors { get; }
        IQueryable<Room> Rooms { get; }
        IQueryable<RoomAssignment> RoomAssignments { get; }
        IQueryable<ColorAssignment> ColorAssignments { get; }

        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove(object entity);
        Task SaveChangesAsync();
    }
}
