using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Booking
    {
        public Booking(long bookingNumber, string firstName, string lastName)
        {
            BookingNumber = bookingNumber;
            FirstName = firstName;
            LastName = lastName;
        }

        public long BookingNumber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; } = Status.New;
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }

    public enum Status {
        New,
        Modified,
        Cancelled
    };
}