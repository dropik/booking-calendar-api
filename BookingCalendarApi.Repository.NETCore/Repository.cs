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
            if (typeof(IStructureData).IsAssignableFrom(typeof(TEntity)))
            {
                ((IStructureData)entity).StructureId = CurrentStructureId;
            }
            return _context.Add(entity).Entity;
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (typeof(IStructureData).IsAssignableFrom(typeof(TEntity)))
            {
                ((IStructureData)entity).StructureId = CurrentStructureId;
            }
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
