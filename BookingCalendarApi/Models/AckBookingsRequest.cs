namespace BookingCalendarApi.Models
{
    public class AckBookingsRequest
    {
        public AckBookingsRequest(string sessionId)
        {
            SessionId = sessionId;
        }

        public IEnumerable<SessionBooking> Bookings { get; set; } = new List<SessionBooking>();
        public string SessionId { get; set; }
    }
}
