using C59Service;

namespace BookingCalendarApi.Services
{
    public interface IC59ServiceSession
    {
        public Task SendAsync(c59WSPO data);
    }
}
