using Newtonsoft.Json;

namespace BookingCalendarApi.Models
{
    public class Tile
    {
        public string Id { get; set; }
        public string BookingId { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public uint Nights { get; set; }
        public string RoomType { get; set; }
        public string Entity { get; set; }
        public uint Persons { get; set; }
        public string Color { get; set; }
        public long? RoomId { get; set; }
    }
}