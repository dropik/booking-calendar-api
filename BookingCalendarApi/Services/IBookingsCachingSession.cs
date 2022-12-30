using BookingCalendarApi.Models;
using Booking = BookingCalendarApi.Models.Iperbooking.Bookings.Booking;

namespace BookingCalendarApi.Services
{
    public interface IBookingsCachingSession
    {
        public Guid Id { get; }
        public Task OpenAsync(string? sessionId);
        public IEnumerable<Booking> ExcludeRooms(IEnumerable<Booking> bookings);
        public void WriteNewData(IEnumerable<SessionBooking> bookings);
    }
}
