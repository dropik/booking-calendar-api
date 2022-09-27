using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Iperbooking
{
    public class Room
    {
        public string RoomName { get; set; } = "";
        public ICollection<RateGroup> RateGroups { get; set; } = new HashSet<RateGroup>();
    }
}
