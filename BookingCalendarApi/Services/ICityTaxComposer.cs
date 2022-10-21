using BookingCalendarApi.Models;
using Booking = BookingCalendarApi.Models.Iperbooking.Bookings.Booking;

namespace BookingCalendarApi.Services
{
    public interface ICityTaxComposer : IComposer<Booking, CityTax> { }
}
