using BookingCalendarApi.Models;
using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi
{
    public class DataContext
    {
        public List<Place> Places { get; private set; } = new();
        public List<Nation> Nations { get; private set; } = new();
        public List<RoomAssignment> RoomAssignments { get; private set; } = new();
    }
}
