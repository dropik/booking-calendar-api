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
        private readonly Func<Reservation, ITileWithClientsComposer> _tileComposerProvider;
        private readonly IIperbooking _iperbooking;
        private readonly BookingCalendarContext _context;

        private List<Booking> Bookings { get; set; } = new();
        private GuestsResponse GuestsResponse { get; set; } = new GuestsResponse();

        public BookingController(
            Func<Reservation, ITileWithClientsComposer> tileComposerProvider,
            IIperbooking iperbooking,
            BookingCalendarContext context)
        {
            _tileComposerProvider = tileComposerProvider;
            _iperbooking = iperbooking;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Booking<List<Client>>>> GetAsync(string id, string from)
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

                var booking = Bookings.SelectById(id);

                var query = (from b in booking
                            join color in _context.ColorAssignments on b.BookingNumber.ToString() equals color.BookingId into gj
                            from color in gj.DefaultIfEmpty()
                            select new { Booking = b, Color = color }
                            ).ToList()
                            .Select(join => new Booking<List<Client>>(
                                id: join.Booking.BookingNumber.ToString(),
                                name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                                lastModified: join.Booking.LastModified,
                                from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"))
                            {
                                Status = join.Booking.Status,
                                Color = join.Color?.Color,
                                Tiles = join.Booking.Rooms
                                    .UseComposer(_tileComposerProvider(
                                        GuestsResponse.Reservations.SingleOrDefault(reservation => reservation.ReservationId == join.Booking.BookingNumber)
                                        ?? throw new Exception("Provided reservation for merging booking with its guests not found")))
                            });

                if (!query.Any())
                {
                    return NotFound();
                }

                return query.First();
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