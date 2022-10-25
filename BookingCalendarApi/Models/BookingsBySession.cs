namespace BookingCalendarApi.Models
{
    public class BookingsBySession
    {
        public BookingsBySession(string sessionId)
        {
            SessionId = sessionId;
        }

        public IEnumerable<Booking<uint>> Bookings { get; set; } = new List<Booking<uint>>();
        public string SessionId { get; set; }
    }
}
