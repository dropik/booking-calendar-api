using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class CalculatorExtensions
    {
        public static CityTax UseCalculator(this IEnumerable<Stay> stays, ICityTaxCalculator calculator)
        {
            return stays
                .Select(stay => calculator.Calculate(stay))
                .GroupBy(x => 1)
                .Select(cityTaxes => new CityTax()
                {
                    Standard = Convert.ToUInt32(cityTaxes.Sum(t => t.Standard)),
                    Children = Convert.ToUInt32(cityTaxes.Sum(t => t.Children)),
                    Over10Days = Convert.ToUInt32(cityTaxes.Sum(t => t.Over10Days))
                })
                .FirstOrDefault(x => true, new CityTax());
        }
    }
}
