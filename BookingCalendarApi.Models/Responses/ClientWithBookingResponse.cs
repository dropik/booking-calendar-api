namespace BookingCalendarApi.Models.Responses
{
    public class ClientWithBookingResponse : ClientResponse
    {
        public string BookingName { get; set; }
        public string BookingFrom { get; set; }
        public string BookingTo { get; set; }

        public ClientWithBookingResponse(string id, string bookingId, string name, string surname, string dateOfBirth, string bookingName, string bookingFrom, string bookingTo)
            : base(id, bookingId, name, surname, dateOfBirth)
        {
            BookingName = bookingName;
            BookingFrom = bookingFrom;
            BookingTo = bookingTo;
        }
    }
}
