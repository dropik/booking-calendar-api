using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Repository.Common;
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

        public IQueryable<Structure> Structures => GetQueryable<Structure>();
        public IQueryable<User> Users => GetQueryable<User>();
        public IQueryable<UserRefreshToken> UserRefreshTokens => GetQueryable<UserRefreshToken>();
        public IQueryable<Nation> Nations => GetQueryable<Nation>();
        public IQueryable<Floor> Floors => GetQueryable<Floor>();
        public IQueryable<Room> Rooms => GetQueryable<Room>();
        public IQueryable<RoomAssignment> RoomAssignments => GetQueryable<RoomAssignment>();
        public IQueryable<ColorAssignment> ColorAssignments => GetQueryable<ColorAssignment>();

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

        private IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if (typeof(IStructureData).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => ((IStructureData)e).StructureId == CurrentStructureId);
            }
            return query;
        }
    }
}
