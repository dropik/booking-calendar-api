namespace BookingCalendarApi.Models
{
    public class TileAssignment
    {
        public TileAssignment(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
        public string Color { get; set; }
        public long? RoomId { get; set; }
    }
}
