using System.Collections.Generic;

namespace BookingCalendarApi.Models.Iperbooking.RoomRates
{
    public class RoomRateRoom
    {
        public RoomRateRoom(string roomName)
        {
            RoomName = roomName;
        }
        
        public string RoomName { get; set; }
        public ICollection<RateGroup> RateGroups { get; set; } = new List<RateGroup>();
    }
}
