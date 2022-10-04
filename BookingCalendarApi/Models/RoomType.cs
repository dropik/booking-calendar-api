namespace BookingCalendarApi.Models
{
    public class RoomType
    {
        public RoomType(string name, uint minOccupancy, uint maxOccupancy)
        {
            Name = name;
            MinOccupancy = minOccupancy;
            MaxOccupancy = maxOccupancy;
        }
        
        public string Name { get; set; }
        public uint MinOccupancy { get; set; }
        public uint MaxOccupancy { get; set; }
    }
}
