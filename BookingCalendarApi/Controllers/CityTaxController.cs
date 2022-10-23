using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/city-tax")]
    [ApiController]
    public class CityTaxController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IAssignedBookingComposer _assignedBookingComposer;
        private readonly IStayComposer _stayComposer;
        private readonly IIperbooking _iperbooking;
        private readonly Func<string, string, IEnumerable<Reservation>, ICityTaxCalculator> _calculatorProvider;

        public CityTaxController(
            IBookingsProvider bookingsProvider,
            IAssignedBookingComposer assignedBookingComposer,
            IStayComposer stayComposer,
            IIperbooking iperbooking,
            Func<string, string, IEnumerable<Reservation>, ICityTaxCalculator> calculatorProvider
        )
        {
            _bookingsProvider = bookingsProvider;
            _assignedBookingComposer = assignedBookingComposer;
            _stayComposer = stayComposer;
            _iperbooking = iperbooking;
            _calculatorProvider = calculatorProvider;
        }

        [HttpGet]
        public async Task<ActionResult<CityTax>> GetAsync(string from, string to)
        {
            try
            {
                await _bookingsProvider.FetchBookingsAsync(from, to);
                var bookings = _bookingsProvider.Bookings
                    .ExcludeCancelled()
                    .SelectInRange(from, to);

                var bookingIds = "";
                foreach (var booking in bookings)
                {
                    bookingIds += $"{booking.BookingNumber},";
                }

                var guestResponse = await _iperbooking.GetGuestsAsync(bookingIds);

                var calculator = _calculatorProvider(from, to, guestResponse.Reservations);

                return bookings
                    .UseComposer(_assignedBookingComposer)
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
