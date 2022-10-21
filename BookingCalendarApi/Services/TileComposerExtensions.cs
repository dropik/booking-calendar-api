using BookingCalendarApi.Models;
using Room = BookingCalendarApi.Models.Iperbooking.Bookings.Room;

namespace BookingCalendarApi.Services
{
    public static class TileComposerExtensions
    {
        public static IEnumerable<Tile> UseComposer(this IEnumerable<Room> rooms, ITileComposer tileComposer)
        {
            return tileComposer.Compose(rooms);
        }
    }
}
