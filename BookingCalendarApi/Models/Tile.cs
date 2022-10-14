using System.Text.Json.Serialization;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Models
{
    public class Tile
    {
        public Tile(
            string id,
            string bookingId,
            string name,
            string from,
            uint nights,
            string roomType,
            string entity,
            uint persons,
            string color
        )
        {
            Id = id;
            BookingId = bookingId;
            Name = name;
            From = from;
            Nights = nights;
            RoomType = roomType;
            Entity = entity;
            Persons = persons;
            Color = color;
        }

        public string Id { get; set; }
        public string BookingId { get; set; }
        [JsonConverter(typeof(LowerCaseEnumConverter))]
        public Status Status { get; set; } = Status.New;
        public string Name { get; set; }
        public string From { get; set; }
        public uint Nights { get; set; }
        public string RoomType { get; set; }
        public string Entity { get; set; }
        public uint Persons { get; set; }
        public string Color { get; set; }
        public long? RoomId { get; set; }
    }

    class LowerCaseEnumConverter : JsonStringEnumConverter
    {
        public LowerCaseEnumConverter() : base(System.Text.Json.JsonNamingPolicy.CamelCase, false) { }
    }
}