using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Repository.NETFramework
{
    public class Repository : IRepository
    {
        private readonly BookingCalendarContext _context;
        private readonly IUserClaimsProvider _claimsProvider;

        public Repository(BookingCalendarContext context, IUserClaimsProvider claimsProvider)
        {
            _context = context;
            _claimsProvider = claimsProvider;
        }

        private long CurrentStructureId => long.Parse(_claimsProvider.User.Claims.FirstOrDefault(c => c.Type == JWT.STRUCTURE_CLAIM)?.Value ?? "0");

        public Common.IDbSet<Structure> Structures => new DbSetWrapper<Structure>(_context.Structures, CurrentStructureId);
        public Common.IDbSet<User> Users => new DbSetWrapper<User>(_context.Users, CurrentStructureId);
        public Common.IDbSet<UserRefreshToken> UserRefreshTokens => new DbSetWrapper<UserRefreshToken>(_context.UserRefreshTokens, CurrentStructureId);
        public Common.IDbSet<Nation> Nations => new DbSetWrapper<Nation>(_context.Nations, CurrentStructureId);
        public Common.IDbSet<Floor> Floors => new DbSetWrapper<Floor>(_context.Floors, CurrentStructureId);
        public Common.IDbSet<Room> Rooms => new DbSetWrapper<Room>(_context.Rooms, CurrentStructureId);
        public Common.IDbSet<RoomAssignment> RoomAssignments => new DbSetWrapper<RoomAssignment>(_context.RoomAssignments, CurrentStructureId);
        public Common.IDbSet<ColorAssignment> ColorAssignments => new DbSetWrapper<ColorAssignment>(_context.ColorAssignments, CurrentStructureId);

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = _context.Add(entity);
            entry.State = EntityState.Added;
            return entry.Entity;
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            return entry.Entity;
        }

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
