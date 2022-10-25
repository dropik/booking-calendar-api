using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly Func<IEnumerable<Reservation>, IBookingWithClientsComposer> _bookingComposerProvider;
        private readonly IIperbooking _iperbooking;

        private GuestsResponse GuestsResponse { get; set; } = new GuestsResponse();

        public BookingController(
            IBookingsProvider bookingsProvider,
            Func<IEnumerable<Reservation>, IBookingWithClientsComposer> bookingComposerProvider,
            IIperbooking iperbooking
        )
        {
            _bookingsProvider = bookingsProvider;
            _bookingComposerProvider = bookingComposerProvider;
            _iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<ActionResult<Booking<IEnumerable<Client>>>> GetAsync(string id, string from)
        {
            try
            {
                var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
                var arrivalFrom = fromDate.AddDays(-15).ToString("yyyy-MM-dd");
                var arrivalTo = fromDate.AddDays(15).ToString("yyyy-MM-dd");

                await Task.WhenAll(
                    _bookingsProvider.FetchBookingsAsync(arrivalFrom, arrivalTo, exactPeriod: true),
                    FetchGuestsAsync(id)
                );
                
                var bookingComposer = _bookingComposerProvider(GuestsResponse.Reservations);

                var result = _bookingsProvider.Bookings
                    .SelectById(id)
                    .UseComposer(bookingComposer);

                if (!result.Any())
                {
                    return NotFound();
                }

                return result.First();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task FetchGuestsAsync(string id)
        {
            GuestsResponse = await _iperbooking.GetGuestsAsync(id);
        }
    }
}