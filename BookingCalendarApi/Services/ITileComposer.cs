using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface ITileComposer
    {
        public Task OpenAsync();
        public IEnumerable<Tile> Compose(IEnumerable<FlattenedRoom> rooms);
    }
}
