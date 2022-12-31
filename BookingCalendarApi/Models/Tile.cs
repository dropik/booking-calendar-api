namespace BookingCalendarApi.Models
{
    public class Tile<TPerson>
    {
        public Tile(
            string id,
            string from,
            uint nights,
            string roomType,
            string rateId,
            TPerson persons
        )
        {
            Id = id;
            From = from;
            Nights = nights;
            RoomType = roomType;
            RateId = rateId;
            Persons = persons;
        }

        public string Id { get; set; }
        public string From { get; set; }
        public uint Nights { get; set; }
        public string RoomType { get; set; }
        public TPerson Persons { get; set; }
        public long? RoomId { get; set; }
        public string RateId { get; set; } = "";
    }
}