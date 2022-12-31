namespace BookingCalendarApi.Models
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
