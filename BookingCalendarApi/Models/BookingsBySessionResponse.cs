namespace BookingCalendarApi.Models
{
    public class BookingsBySessionResponse
    {
        public BookingsBySessionResponse(string sessionId)
        {
            SessionId = sessionId;
        }

        public List<Booking<uint>> Bookings { get; set; } = new List<Booking<uint>>();
        public string SessionId { get; set; }
    }
}
