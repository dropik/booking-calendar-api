using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Repository.NETFramework
{
    public class Repository : IRepository
    {
        private readonly BookingCalendarContext _context;

        public Repository(BookingCalendarContext context)
        {
            _context = context;
        }

        public Common.IDbSet<Structure> Structures => new DbSetWrapper<Structure>(_context.Structures);
        public Common.IDbSet<User> Users => new DbSetWrapper<User>(_context.Users);
        public Common.IDbSet<UserRefreshToken> UserRefreshTokens => new DbSetWrapper<UserRefreshToken>(_context.UserRefreshTokens);
        public Common.IDbSet<Nation> Nations => new DbSetWrapper<Nation>(_context.Nations);
        public Common.IDbSet<Floor> Floors => new DbSetWrapper<Floor>(_context.Floors);
        public Common.IDbSet<Room> Rooms => new DbSetWrapper<Room>(_context.Rooms);
        public Common.IDbSet<RoomAssignment> RoomAssignments => new DbSetWrapper<RoomAssignment>(_context.RoomAssignments);
        public Common.IDbSet<ColorAssignment> ColorAssignments => new DbSetWrapper<ColorAssignment>(_context.ColorAssignments);

        public async Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.ToListAsync();

        public async Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey, TEntity>(IQueryable<TEntity> entities, Func<TEntity, TKey> keySelector)
            => await entities.ToDictionaryAsync(keySelector);

        public async Task<TEntity> SingleAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.SingleAsync();

        public async Task<TEntity> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.SingleOrDefaultAsync();

        public async Task<bool> AnyAsync<TEntity>(IQueryable<TEntity> entities)
            => await entities.AnyAsync();

        public void Add(object entity)
            => _context.Entry(entity).State = EntityState.Added;

        public void Update(object entity)
            => _context.Entry(entity).State = EntityState.Modified;

        public void Remove(object entity)
            => _context.Entry(entity).State = EntityState.Deleted;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            var entries = _context.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
