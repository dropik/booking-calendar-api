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
        private readonly Func<ICityTaxCalculator> _calculatorProvider;

        public CityTaxController(
            IBookingsProvider bookingsProvider,
            IStayComposer stayComposer,
            Func<ICityTaxCalculator> calculatorProvider
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
                var calculator = _calculatorProvider();

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
