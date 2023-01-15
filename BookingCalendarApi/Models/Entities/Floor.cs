namespace BookingCalendarApi.Models.Entities
{
    public class Floor
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";

        public List<Room> Rooms { get; set; } = new();
    }
}
