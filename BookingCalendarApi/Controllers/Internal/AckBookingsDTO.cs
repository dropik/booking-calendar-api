using BookingCalendarApi.Models;

namespace BookingCalendarApi.Controllers.Internal
{
    public class AckBookingsDTO
    {
        public AckBookingsDTO(string sessionId)
        {
            SessionId = sessionId;
        }

        public IEnumerable<SessionBooking> Bookings { get; set; } = new List<SessionBooking>();
        public string SessionId { get; set; }
    }
}
