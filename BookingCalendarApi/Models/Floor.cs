namespace BookingCalendarApi.Models
{
    public class Floor
    {
        public string Name { get; set; } = "";
        public ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
    }
}
