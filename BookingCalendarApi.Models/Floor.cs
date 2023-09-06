using System.Collections.Generic;

namespace BookingCalendarApi.Models
{
    public class Floor
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";

        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
