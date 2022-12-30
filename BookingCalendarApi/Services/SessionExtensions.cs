using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class SessionExtensions
    {
        public static IEnumerable<Booking> ExcludeBySession(this IEnumerable<Booking> bookings, IBookingsCachingSession session)
        {
            return session.ExcludeRooms(bookings);
        }
    }
}
