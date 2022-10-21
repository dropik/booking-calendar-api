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

        public CityTax Calculate(Stay stay)
        {
            var value = _wrappee.Calculate(stay);
            return DecorateCalculation(stay, value);
        }

        protected abstract CityTax DecorateCalculation(Stay stay, CityTax value);
    }
}
