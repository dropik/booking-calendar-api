using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class Session : ISession
    {
        private readonly BookingCalendarContext _context;

        public Guid Id { get; private set; }
        private IEnumerable<SessionItem> SessionItems { get; set; } = new List<SessionItem>();

        public Session(BookingCalendarContext context)
        {
            _context = context;
        }

        public async Task OpenAsync(string? sessionId)
        {
            Id = GetGuid(sessionId);
            SessionItems = await _context.SessionMap.Where(session => session.Id.Equals(Id)).ToListAsync();
        }

        private Guid GetGuid(string? sessionId)
        {
            try
            {
                return sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();
            } catch (Exception)
            {
                return Guid.NewGuid();
            }
        }
        
        public IEnumerable<FlattenedRoom> ExcludeRooms(IEnumerable<FlattenedRoom> rooms)
        {
            foreach (var room in rooms)
            {
                var newSession = new SessionItem(Id, room.Id, room.Booking.LastModified);
                var sessionItemQuery = SessionItems.Where(session => session.Id.Equals(Id) && session.TileId == room.Id);
                if (sessionItemQuery.Any())
                {
                    var sessionItem = sessionItemQuery.First();
                    if (sessionItem.LastModified == room.Booking.LastModified)
                    {
                        continue;
                    }
                    _context.Entry(newSession).State = EntityState.Modified;
                } else
                {
                    _context.SessionMap.Add(newSession);
                }

                yield return room;
            }
        }

        public async Task CloseAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}