using BookingCalendarApi.Models.Iperbooking.Bookings;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Controllers.Internal
{
    public class ResponseBooking
    {
        public ResponseBooking(string id, string name, string lastModified, string from, string to)
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

        public IEnumerable<Tile> Tiles { get; set; } = new List<Tile>();

        class LowerCaseEnumConverter : JsonStringEnumConverter
        {
            public LowerCaseEnumConverter() : base(System.Text.Json.JsonNamingPolicy.CamelCase, false) { }
        }

    }
}
