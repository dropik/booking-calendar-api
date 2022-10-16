using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingColorizer
    {
        public IEnumerable<ColorizedBooking> Colorize(IEnumerable<Booking> bookings);
    }
}
