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
        public Status Status { get; set; } = Status.New;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastModified { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }

    public enum Status {
        New,
        Modified,
        Cancelled
    };
}