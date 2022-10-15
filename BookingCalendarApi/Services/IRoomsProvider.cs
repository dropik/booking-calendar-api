namespace BookingCalendarApi.Services
{
    public interface IRoomsProvider
    {
        public Task AccumulateAllRoomsAsync(string from, string to);
        public IEnumerable<FlattenedRoom> Rooms { get; }
    }
}
