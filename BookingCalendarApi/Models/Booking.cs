using BookingCalendarApi.Models.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models
{
    public class Booking<TPerson>
    {
        public Booking(string id, string name, string lastModified, string from, string to)
        {
            Id = id;
            Name = name;
            LastModified = lastModified;
            From = from;
            To = to;
        }

        public string Id { get; set; }
        [JsonConverter(typeof(LowerCaseEnumConverter))]
        public BookingStatus Status { get; set; } = BookingStatus.New;
        public string Name { get; set; }
        public string LastModified { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string? Color { get; set; }

        public IEnumerable<Tile<TPerson>> Tiles { get; set; } = new List<Tile<TPerson>>();
    }
}
