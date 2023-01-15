using BookingCalendarApi.Models.Entities.EntityContents;

namespace BookingCalendarApi.Models.Requests
{
    public class AckBookingsRequest
    {
        public AckBookingsRequest(string sessionId)
        {
            SessionId = sessionId;
        }

        public List<SessionBooking> Bookings { get; set; } = new();
        public string SessionId { get; set; }
    }
}
