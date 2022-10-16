using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingComposer
    {
        public IEnumerable<ResponseBooking> Compose(IEnumerable<Booking> bookings);
    }
}
