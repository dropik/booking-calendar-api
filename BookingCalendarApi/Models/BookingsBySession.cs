namespace BookingCalendarApi.Models
{
    public class BookingsBySession
    {
        public BookingsBySession(string sessionId)
        {
            SessionId = sessionId;
        }

        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
        public string SessionId { get; set; }
    }
}
