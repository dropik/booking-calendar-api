namespace BookingCalendarApi.Models.Iperbooking.RoomRates
{
    public class Room
    {
        public Room(string roomName)
        {
            RoomName = roomName;
        }
        
        public string RoomName { get; set; }
        public ICollection<RateGroup> RateGroups { get; set; } = new List<RateGroup>();
    }
}
