namespace BookingCalendarApi.Models
{
    public class RoomType
    {
        public string Name { get; set; } = "";
        public uint MinOccupancy { get; set; }
        public uint MaxOccupancy { get; set; }
    }
}
