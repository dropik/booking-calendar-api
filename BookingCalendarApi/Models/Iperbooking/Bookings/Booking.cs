namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Booking
    {
        public string BookingNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}