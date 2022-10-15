namespace BookingCalendarApi.Services
{
    public interface IRoomsProvider
    {
        public Task<IEnumerable<FlattenedRoom>> AccumulateAllRoomsAsync(string from, string to);
    }
}
