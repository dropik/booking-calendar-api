using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class BookingComposerExtensions
    {
        public static IEnumerable<ResponseBooking> UseComposer(this IEnumerable<Booking> bookings, IBookingComposer composer)
        {
            return composer.Compose(bookings);
        }
    }
}
