namespace BookingCalendarApi.Services
{
    public interface ISession
    {
        public Task OpenAsync();
        public IEnumerable<FlattenedRoom> ExcludeRooms(IEnumerable<FlattenedRoom> rooms);
        public Task CloseAsync();
    }
}
