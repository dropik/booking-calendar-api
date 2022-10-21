using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/city-tax")]
    [ApiController]
    public class CityTaxController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IStayComposer _stayComposer;
        private readonly ICityTaxComposer _cityTaxComposer;

        public CityTaxController(IBookingsProvider bookingsProvider, IStayComposer stayComposer, ICityTaxComposer cityTaxComposer)
        {
            _bookingsProvider = bookingsProvider;
            _stayComposer = stayComposer;
            _cityTaxComposer = cityTaxComposer;
        }

        [HttpGet]
        public async Task<ActionResult<CityTax>> GetAsync(string from, string to)
        {
            try
            {
                await _bookingsProvider.FetchBookingsAsync(from, to);

                return _bookingsProvider.Bookings
                    .ExcludeCancelled()
                    .SelectInRange(from, to)
                    .UseComposer(_stayComposer)
                    .ExcludeNotAssigned()
                    .UseComposer(_cityTaxComposer)
                    .FirstOrDefault(x => true, new CityTax());
                    
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
