using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IBookingColorizer _bookingColorizer;
        private readonly Func<Services.ISession> _sessionProvider;
        private readonly ITileComposer _tileComposer;

        public TilesController(
            BookingCalendarContext context,
            IBookingsProvider bookingsProvider,
            IBookingColorizer bookingColorizer,
            Func<Services.ISession> sessionProvider,
            ITileComposer tileComposer
        )
        {
            _context = context;
            _bookingsProvider = bookingsProvider;
            _bookingColorizer = bookingColorizer;
            _sessionProvider = sessionProvider;
            _tileComposer = tileComposer;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();

                await Task.WhenAll(
                    session.OpenAsync(sessionId),
                    _bookingsProvider.FetchBookingsAsync(from, to)
                );

                var tiles = _bookingsProvider.Bookings
                    .UseColorizer(_bookingColorizer)
                    .SelectInRangeRooms(from, to)
                    .ExcludeBySession(session)
                    .UseComposer(_tileComposer)
                    .ToList();

                return new TileResponse(session.Id.ToString())
                {
                    Tiles = tiles
                };
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
