namespace BookingCalendarApi.Services
{
    public class FlattenedRoom
    {
        public FlattenedRoom(
            string id,
            DateTime from,
            DateTime to,
            Models.Iperbooking.Bookings.Booking booking,
            Models.Iperbooking.Bookings.Room room
        )
        {
            Id = id;
            From = from;
            To = to;
            Booking = booking;
            Room = room;
        }

        public string Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Models.Iperbooking.Bookings.Booking Booking { get; set; }
        public Models.Iperbooking.Bookings.Room Room { get; set; }
    }
}
