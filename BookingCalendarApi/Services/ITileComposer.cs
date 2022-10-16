using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ITileComposer
    {
        public IEnumerable<Tile> Compose(IEnumerable<Room> rooms);
    }
}
