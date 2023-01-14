using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class AckBookingsService : IAckBookingsService
    {
        private readonly IBookingsCachingSession _session;
        private readonly BookingCalendarContext _context;

        public AckBookingsService(IBookingsCachingSession session, BookingCalendarContext context)
        {
            _session = session;
            _context = context;
        }

        public async Task Ack(AckBookingsRequest request)
        {
            var bookings = request.Bookings;
            var sessionId = request.SessionId;

            if (bookings == null)
            {
                return;
            }

            await _session.OpenAsync(sessionId);
            _session.WriteNewData(bookings);
            await _context.SaveChangesAsync();
        }
    }
}
