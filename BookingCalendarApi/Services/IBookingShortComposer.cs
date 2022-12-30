using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingShortComposer : IComposer<Booking, ShortBooking> { }
}
