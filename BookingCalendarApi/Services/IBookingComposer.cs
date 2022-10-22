using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingComposer : IComposer<Models.Iperbooking.Bookings.Booking, Models.Booking> { }
}
