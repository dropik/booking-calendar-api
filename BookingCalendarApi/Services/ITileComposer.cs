using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface ITileComposer
    {
        public IEnumerable<Tile> Compose(IEnumerable<FlattenedRoom> rooms);
    }
}
