using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public abstract class CityTaxCalculatorDecoratorBase : ICityTaxCalculator
    {
        private readonly ICityTaxCalculator _wrappee;

        public CityTaxCalculatorDecoratorBase(ICityTaxCalculator wrappee)
        {
            _wrappee = wrappee;
        }

        public virtual CityTax Calculate(Stay stay)
        {
            return _wrappee.Calculate(stay);
        }
    }
}
