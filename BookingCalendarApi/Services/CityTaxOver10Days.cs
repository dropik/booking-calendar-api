using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class CityTaxOver10Days : CityTaxCalculatorDecoratorBase
    {
        public CityTaxOver10Days(ICityTaxCalculator wrappee) : base(wrappee) { }

        public override CityTax Calculate(Stay stay)
        {
            var value = base.Calculate(stay);
            var arrival = DateTime.ParseExact(stay.Arrival, "yyyyMMdd", null);
            var departure = DateTime.ParseExact(stay.Departure, "yyyyMMdd", null);
            var nights = (departure - arrival).Days;
            var standardGuests = value.Standard / nights;
            var standardNights = nights <= 10 ? nights : 10;
            var prevStandard = value.Standard;
            value.Standard = (uint)(standardGuests * standardNights);
            value.Over10Days = prevStandard - value.Standard;
            return value;
        }
    }
}
