using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly Func<IEnumerable<Reservation>, IBookingWithClientsComposer> _bookingComposerProvider;
        private readonly IIperbooking _iperbooking;

        private List<Booking> Bookings { get; set; } = new();
        private GuestsResponse GuestsResponse { get; set; } = new GuestsResponse();

        public BookingController(
            Func<IEnumerable<Reservation>, IBookingWithClientsComposer> bookingComposerProvider,
            IIperbooking iperbooking
        )
        {
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
                    FetchBookings(arrivalFrom, arrivalTo),
                    FetchGuests(id)
                );
                
                var bookingComposer = _bookingComposerProvider(GuestsResponse.Reservations);

                var result = Bookings
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

        private async Task FetchBookings(string from, string to)
        {
            Bookings = await _iperbooking.GetBookingsAsync(from, to, exactPeriod: true);
        }

        private async Task FetchGuests(string id)
        {
            GuestsResponse = await _iperbooking.GetGuestsAsync(id);
        }
    }
}