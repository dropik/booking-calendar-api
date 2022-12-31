namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class BookingRoom<TGuest>
    {
        public BookingRoom(long stayId, string roomName, string arrival, string departure, string rateId)
        {
            StayId = stayId;
            RoomName = roomName;
            Arrival = arrival;
            Departure = departure;
            RateId = rateId;
        }
        
        public long StayId { get; set; }
        public string RoomName { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string RateId { get; set; } = "";
        
        public List<TGuest> Guests { get; set; } = new();
    }
}
