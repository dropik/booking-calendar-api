namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Room
    {
        public Room(long stayId, string roomName, string arrival, string departure)
        {
            StayId = stayId;
            RoomName = roomName;
            Arrival = arrival;
            Departure = departure;
        }
        
        public long StayId { get; set; }
        public string RoomName { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    }
}
