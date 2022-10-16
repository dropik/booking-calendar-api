using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class BookingColorizerExtensions
    {
        public static IEnumerable<ColorizedBooking> UseColorizer(this IEnumerable<Booking> bookings, IBookingColorizer colorizer)
        {
            return colorizer.Colorize(bookings);
        }
    }
}
