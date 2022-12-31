namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class AssignedBooking<TGuest>
    {
        public AssignedBooking(Booking booking)
        {
            Booking = booking;
        }

        public Booking Booking { get; set; }
        public List<AssignedRoom<TGuest>> Rooms { get; set; } = new();
    }
}
