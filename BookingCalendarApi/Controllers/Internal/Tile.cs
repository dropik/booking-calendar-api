namespace BookingCalendarApi.Controllers.Internal
{
    public class Tile
    {
        public Tile(
            string id,
            string from,
            uint nights,
            string roomType,
            string entity,
            uint persons
        )
        {
            Id = id;
            From = from;
            Nights = nights;
            RoomType = roomType;
            Entity = entity;
            Persons = persons;
        }

        public string Id { get; set; }
        public string From { get; set; }
        public uint Nights { get; set; }
        public string RoomType { get; set; }
        public string Entity { get; set; }
        public uint Persons { get; set; }
        public long? RoomId { get; set; }
    }
}