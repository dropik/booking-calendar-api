using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public static class TileComposerExtensions
    {
        public static IEnumerable<Tile> UseComposer(this IEnumerable<FlattenedRoom> rooms, ITileComposer tileComposer)
        {
            return tileComposer.Compose(rooms);
        }
    }
}
