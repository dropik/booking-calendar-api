using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingsProvider
    {
        public Task FetchBookingsAsync(string from, string to);
        public IEnumerable<Booking> Bookings { get; }
    }
}
