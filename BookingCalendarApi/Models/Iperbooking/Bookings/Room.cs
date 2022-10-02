namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Room
    {
        public string StayId { get; set; }
        public string RoomName { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        
        public ICollection<Guest> Guests { get; set; }
    }
}
