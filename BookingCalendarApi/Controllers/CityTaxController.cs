using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/city-tax")]
    public class CityTaxController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly ICityTaxComposer _cityTaxComposer;

        public CityTaxController(IBookingsProvider bookingsProvider, ICityTaxComposer cityTaxComposer)
        {
            _bookingsProvider = bookingsProvider;
            _cityTaxComposer = cityTaxComposer;
        }

        [HttpGet]
        public async Task<ActionResult<CityTax>> GetAsync(string from, string to)
        {
            try
            {
                await _bookingsProvider.FetchBookingsAsync(from, to);

                return _bookingsProvider.Bookings
                    .SelectInRange(from, to)
                    .UseComposer(_cityTaxComposer)
                    .GroupBy(x => 1)
                    .Select(cityTaxes => new CityTax()
                    {
                        Standard = Convert.ToUInt32(cityTaxes.Sum(t => t.Standard))
                    })
                    .FirstOrDefault(x => true, new CityTax());
                    
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
