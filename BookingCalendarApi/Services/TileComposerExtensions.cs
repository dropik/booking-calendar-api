using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;

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
