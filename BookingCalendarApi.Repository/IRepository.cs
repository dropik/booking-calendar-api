using System;
using System.Collections.Generic;
using System.Linq;
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

        Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> entities);
        Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey, TEntity>(IQueryable<TEntity> entities, Func<TEntity, TKey> keySelector);
        Task<TEntity> SingleAsync<TEntity>(IQueryable<TEntity> entities);
        Task<TEntity> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> entities);
        Task<bool> AnyAsync<TEntity>(IQueryable<TEntity> entities);
        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove(object entity);
        Task SaveChangesAsync();
    }
}
