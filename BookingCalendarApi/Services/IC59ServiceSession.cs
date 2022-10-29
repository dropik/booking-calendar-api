using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IC59ServiceSession
    {
        public Task<IEnumerable<MovementsTestResponseItem>> SendNewDataAsync();
        public Task<string> GetLastDateAsync();
    }
}
