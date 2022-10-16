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
        private readonly Func<Services.ISession> _sessionProvider;
        private readonly Func<ITileComposer> _tileComposerProvider;

        public TilesController(
            BookingCalendarContext context,
            IBookingsProvider bookingsProvider,
            Func<Services.ISession> sessionProvider,
            Func<ITileComposer> tileComposerProvider
        )
        {
            _context = context;
            _bookingsProvider = bookingsProvider;
            _sessionProvider = sessionProvider;
            _tileComposerProvider = tileComposerProvider;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();
                var tileComposer = _tileComposerProvider();

                await Task.WhenAll(
                    session.OpenAsync(sessionId),
                    _bookingsProvider.FetchBookingsAsync(from, to)
                );

                var tiles = _bookingsProvider.Bookings
                    .SelectInRangeRooms(from, to)
                    .ExcludeBySession(session)
                    .UseComposer(tileComposer)
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
