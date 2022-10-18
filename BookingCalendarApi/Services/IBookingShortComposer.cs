using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingShortComposer
    {
        public IEnumerable<BookingShort> Compose(IEnumerable<Booking> bookings);
    }
}
