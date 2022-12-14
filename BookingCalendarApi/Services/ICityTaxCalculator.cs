using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ICityTaxCalculator
    {
        public CityTax Calculate(Stay stay);
    }
}
