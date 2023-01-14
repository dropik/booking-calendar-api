using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class SimpleCityTaxCalculator : ICityTaxCalculator
    {
        public CityTaxResponse Calculate(Stay stay)
        {
            return new CityTaxResponse()
            {
                Standard = Convert.ToUInt32(
                            stay.Guests.Count() *
                            (DateTime.ParseExact(stay.Departure, "yyyyMMdd", null) - DateTime.ParseExact(stay.Arrival, "yyyyMMdd", null)).Days
                        )
            };
        }
    }
}
