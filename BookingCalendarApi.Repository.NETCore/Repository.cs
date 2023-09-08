using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Repository.NETCore
{
    public class Repository : IRepository
    {
        private readonly BookingCalendarContext _context;

        public Repository(BookingCalendarContext context)
        {
            _context = context;
        }

        public IDbSet<Structure> Structures => new DbSetWrapper<Structure>(_context.Structures);
        public IDbSet<User> Users => new DbSetWrapper<User>(_context.Users);
        public IDbSet<UserRefreshToken> UserRefreshTokens => new DbSetWrapper<UserRefreshToken>(_context.UserRefreshTokens);
        public IDbSet<Nation> Nations => new DbSetWrapper<Nation>(_context.Nations);
        public IDbSet<Floor> Floors => new DbSetWrapper<Floor>(_context.Floors);
        public IDbSet<Room> Rooms => new DbSetWrapper<Room>(_context.Rooms);
        public IDbSet<RoomAssignment> RoomAssignments => new DbSetWrapper<RoomAssignment>(_context.RoomAssignments);
        public IDbSet<ColorAssignment> ColorAssignments => new DbSetWrapper<ColorAssignment>(_context.ColorAssignments);

        public async Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.ToListAsync();

        public async Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey, TEntity>(IQueryable<TEntity> entities, Func<TEntity, TKey> keySelector)
            where TKey : notnull
            => await entities.ToDictionaryAsync(keySelector);

        public async Task<TEntity> SingleAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.SingleAsync();

        public async Task<TEntity?> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.SingleOrDefaultAsync();

        public async Task<bool> AnyAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.AnyAsync();

        public void Add(object entity)
            => _context.Add(entity);

        public void Update(object entity)
        {
            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(object entity)
            => _context.Remove(entity);

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
