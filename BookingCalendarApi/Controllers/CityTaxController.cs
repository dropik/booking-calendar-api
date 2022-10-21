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
        private readonly Func<string, string, ICityTaxCalculator> _calculatorProvider;

        public CityTaxController(
            IBookingsProvider bookingsProvider,
            IStayComposer stayComposer,
            Func<string, string, ICityTaxCalculator> calculatorProvider
        )
        {
            _bookingsProvider = bookingsProvider;
            _stayComposer = stayComposer;
            _calculatorProvider = calculatorProvider;
        }

        [HttpGet]
        public async Task<ActionResult<CityTax>> GetAsync(string from, string to)
        {
            try
            {
                await _bookingsProvider.FetchBookingsAsync(from, to);
                var calculator = _calculatorProvider(from, to);

                return _bookingsProvider.Bookings
                    .ExcludeCancelled()
                    .SelectInRange(from, to)
                    .UseComposer(_stayComposer)
                    .ExcludeNotAssigned()
                    .UseCalculator(calculator);
                    
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
