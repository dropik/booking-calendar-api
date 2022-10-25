namespace BookingCalendarApi.Models
{
    public class ClientWithBooking : Client
    {
        public ClientWithBooking(string id, string bookingId, string name, string surname, string dateOfBirth, string bookingName, string bookingFrom, string bookingTo)
            : base(id, bookingId, name, surname, dateOfBirth)
        {
            BookingName = bookingName;
            BookingFrom = bookingFrom;
            BookingTo = bookingTo;
        }

        public string BookingName { get; set; }
        public string BookingFrom { get; set; }
        public string BookingTo { get; set; }
    }
}
