namespace BookingCalendarApi.Controllers.Internal
{
    public class BookingsControllerResponse
    {
        public BookingsControllerResponse(string sessionId)
        {
            SessionId = sessionId;
        }

        public IEnumerable<ResponseBooking> Bookings { get; set; } = new List<ResponseBooking>();
        public string SessionId { get; set; }
    }
}
