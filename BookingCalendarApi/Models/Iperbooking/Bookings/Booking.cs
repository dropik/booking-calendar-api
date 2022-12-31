using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Booking
    {
        public Booking(long bookingNumber, string firstName, string lastName, string lastModified)
        {
            BookingNumber = bookingNumber;
            FirstName = firstName;
            LastName = lastName;
            LastModified = lastModified;
        }

        public long BookingNumber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingStatus Status { get; set; } = BookingStatus.New;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastModified { get; set; }

        public List<BookingRoom<BookingGuest>> Rooms { get; set; } = new();
    }

    public enum BookingStatus {
        New,
        Modified,
        Cancelled
    };
}