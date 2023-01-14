using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IC59ServiceSession
    {
        public Task SendNewDataAsync(DateTime date);
        public Task<IstatLastDateResponse> GetLastDateAsync();
    }
}
