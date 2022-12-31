namespace BookingCalendarApi.Models
{
    public class BookingsBySession
    {
        public BookingsBySession(string sessionId)
        {
            SessionId = sessionId;
        }

        public List<Booking<uint>> Bookings { get; set; } = new List<Booking<uint>>();
        public string SessionId { get; set; }
    }
}
