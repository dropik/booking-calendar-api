using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using Microsoft.EntityFrameworkCore;
using System.Formats.Cbor;

namespace BookingCalendarApi.Services
{
    public class Session : ISession
    {
        private readonly BookingCalendarContext _context;

        public Guid Id { get; private set; }
        private List<SessionBooking> SessionBookings { get; set; } = new List<SessionBooking>();
        private SessionEntry? Entry { get; set; }

        public Session(BookingCalendarContext context)
        {
            _context = context;
        }

        public async Task OpenAsync(string? sessionId)
        {
            Id = GetGuid(sessionId);
            SessionBookings = new List<SessionBooking>();
            Entry = await _context.Sessions.SingleOrDefaultAsync(session => session.Id.Equals(Id));
            if (Entry != null && !Entry.Id.Equals(Guid.Empty))
            {
                var data = Entry.Data;
                var reader = new CborReader(data);
                reader.ReadStartArray();
                while (reader.PeekState() != CborReaderState.EndArray)
                {
                    var bookingId = reader.ReadTextString();
                    var lastModified = reader.ReadTextString();
                    SessionBookings.Add(new SessionBooking(bookingId, lastModified));
                }
            }
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
        
        public IEnumerable<Booking> ExcludeRooms(IEnumerable<Booking> bookings)
        {
            return bookings
                .Where(booking => !SessionBookings
                    .Where(sessionBooking => sessionBooking.Equals(new SessionBooking(booking.BookingNumber.ToString(), booking.LastModified)))
                    .Any());
        }
    }
}