using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class CityTaxComposer : ICityTaxComposer
    {
        public IEnumerable<CityTax> Compose(IEnumerable<Stay> source)
        {
            return source
                .Select(stay => new CityTax()
                    {
                        Standard = Convert.ToUInt32(
                            stay.Guests.Count() *
                            (DateTime.ParseExact(stay.Departure, "yyyyMMdd", null) - DateTime.ParseExact(stay.Arrival, "yyyyMMdd", null)).Days
                        )
                    }
                )
                .GroupBy(x => 1)
                .Select(cityTaxes => new CityTax()
                {
                    Standard = Convert.ToUInt32(cityTaxes.Sum(t => t.Standard))
                });
        }
    }
}
