using BookingCalendarApi.Repository.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Repository.NETCore
{
    public class Repository : IRepository
    {
        private readonly BookingCalendarContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public Repository(BookingCalendarContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private long CurrentStructureId => long.Parse(_contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "StructureId")?.Value ?? "0");

        public IDbSet<Structure> Structures => new DbSetWrapper<Structure>(_context.Structures, CurrentStructureId);
        public IDbSet<User> Users => new DbSetWrapper<User>(_context.Users, CurrentStructureId);
        public IDbSet<UserRefreshToken> UserRefreshTokens => new DbSetWrapper<UserRefreshToken>(_context.UserRefreshTokens, CurrentStructureId);
        public IDbSet<Nation> Nations => new DbSetWrapper<Nation>(_context.Nations, CurrentStructureId);
        public IDbSet<Floor> Floors => new DbSetWrapper<Floor>(_context.Floors, CurrentStructureId);
        public IDbSet<Room> Rooms => new DbSetWrapper<Room>(_context.Rooms, CurrentStructureId);
        public IDbSet<RoomAssignment> RoomAssignments => new DbSetWrapper<RoomAssignment>(_context.RoomAssignments, CurrentStructureId);
        public IDbSet<ColorAssignment> ColorAssignments => new DbSetWrapper<ColorAssignment>(_context.ColorAssignments, CurrentStructureId);

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
            => _context.Add(entity).Entity;

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = _context.Attach(entity);
            entry.State = EntityState.Modified;
            return entry.Entity;
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
