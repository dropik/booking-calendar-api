using C59Service;

namespace BookingCalendarApi.Services
{
    public interface IC59ServiceSession
    {
        public Task SendNewDataAsync();
        public Task<string> GetLastDateAsync();
    }
}
