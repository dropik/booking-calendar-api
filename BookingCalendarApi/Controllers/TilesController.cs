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
        private readonly IRoomsProvider _roomsProvider;
        private readonly Func<Services.ISession> _sessionProvider;
        private readonly Func<ITileComposer> _tileComposerProvider;
        private readonly Func<Func<Task>, Func<Task>, IAsyncScheduler> _asyncSchedulerProvider;

        public TilesController(
            BookingCalendarContext context,
            IRoomsProvider roomsProvider,
            Func<Services.ISession> sessionProvider,
            Func<ITileComposer> tileComposerProvider,
            Func<Func<Task>, Func<Task>, IAsyncScheduler> asyncSchedulerProvider
        )
        {
            _context = context;
            _roomsProvider = roomsProvider;
            _sessionProvider = sessionProvider;
            _tileComposerProvider = tileComposerProvider;
            _asyncSchedulerProvider = asyncSchedulerProvider;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();
                var tileComposer = _tileComposerProvider();

                var scheduler = _asyncSchedulerProvider(
                    async () =>
                    {
                        await session.OpenAsync(sessionId);
                        await tileComposer.OpenAsync();
                    },
                    async () => await _roomsProvider.AccumulateAllRoomsAsync(from, to)
                );

                await scheduler.Execute();

                var tiles = _roomsProvider.Rooms
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
