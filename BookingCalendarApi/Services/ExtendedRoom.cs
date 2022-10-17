using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class ExtendedRoom
    {
        public ExtendedRoom(
            string id,
            DateTime from,
            DateTime to,
            Room room
        )
        {
            Id = id;
            From = from;
            To = to;
            Room = room;
        }

        public string Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Room Room { get; set; }
    }
}
