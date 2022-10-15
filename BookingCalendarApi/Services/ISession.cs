namespace BookingCalendarApi.Services
{
    public interface ISession
    {
        public Guid Id { get; }
        public Task OpenAsync(string? sessionId);
        public IEnumerable<FlattenedRoom> ExcludeRooms(IEnumerable<FlattenedRoom> rooms);
    }
}
