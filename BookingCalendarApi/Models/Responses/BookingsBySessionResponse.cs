namespace BookingCalendarApi.Models.Responses
{
    public class BookingsBySessionResponse
    {
        public BookingsBySessionResponse(string sessionId)
        {
            SessionId = sessionId;
        }

        public List<BookingResponse<uint>> Bookings { get; set; } = new List<BookingResponse<uint>>();
        public string SessionId { get; set; }
    }
}
