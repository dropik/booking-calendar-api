using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IRoomsProvider _roomsProvider;
        private readonly Func<Services.ISession> _sessionProvider;
        private readonly Func<ITileComposer> _tileComposerProvider;

        public TilesController(BookingCalendarContext context, IRoomsProvider roomsProvider, Func<Services.ISession> sessionProvider, Func<ITileComposer> tileComposerProvider)
        {
            _context = context;
            _roomsProvider = roomsProvider;
            _sessionProvider = sessionProvider;
            _tileComposerProvider = tileComposerProvider;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();
                await session.OpenAsync(sessionId);

                var tileComposer = _tileComposerProvider();
                await tileComposer.OpenAsync();

                await _roomsProvider.AccumulateAllRoomsAsync(from, to);
                var rooms = _roomsProvider.Rooms;

                var tiles = rooms
                    .ExcludeBySession(session)
                    .UseComposer(tileComposer)
                    .ToList();

                await _context.SaveChangesAsync();

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
