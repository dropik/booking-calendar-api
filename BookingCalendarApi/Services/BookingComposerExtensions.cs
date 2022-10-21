using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class BookingComposerExtensions
    {
        public static IEnumerable<TOut> UseComposer<TOut>(this IEnumerable<Booking> bookings, IComposer<Booking, TOut> composer)
        {
            return composer.Compose(bookings);
        }
    }
}
