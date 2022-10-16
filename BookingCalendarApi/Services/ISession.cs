using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ISession
    {
        public Guid Id { get; }
        public Task OpenAsync(string? sessionId);
        public IEnumerable<Booking> ExcludeRooms(IEnumerable<Booking> bookings);
    }
}
