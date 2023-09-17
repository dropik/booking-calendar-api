using System.Threading.Tasks;
using BookingCalendarApi.Repository.Common;

namespace BookingCalendarApi.Repository
{
    public interface IRepository
    {
        IDbSet<Structure> Structures { get; }
        IDbSet<User> Users { get; }
        IDbSet<UserRefreshToken> UserRefreshTokens { get; }
        IDbSet<Nation> Nations { get; }
        IDbSet<Floor> Floors { get; }
        IDbSet<Room> Rooms { get; }
        IDbSet<RoomAssignment> RoomAssignments { get; }
        IDbSet<ColorAssignment> ColorAssignments { get; }

        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove(object entity);
        Task SaveChangesAsync();
    }
}
