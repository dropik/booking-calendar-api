using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingsProvider
    {
        public Task FetchBookingsAsync(string from, string to, bool exactPeriod = false);
        public IEnumerable<Booking> Bookings { get; }
    }
}
