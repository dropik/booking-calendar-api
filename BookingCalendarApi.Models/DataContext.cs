using BookingCalendarApi.Models;
using BookingCalendarApi.Models.AlloggiatiService;
using System.Collections.Generic;

namespace BookingCalendarApi
{
    public class DataContext
    {
        public List<Place> Places { get; private set; } = new List<Place>();
        public List<Nation> Nations { get; private set; } = new List<Nation>();
        public List<RoomAssignment> RoomAssignments { get; private set; } = new List<RoomAssignment>();
    }
}
