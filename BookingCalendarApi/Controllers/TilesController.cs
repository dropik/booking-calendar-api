using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IRoomsProvider _roomsProvider;
        private readonly Func<Services.ISession> _sessionProvider;

        public TilesController(BookingCalendarContext context, IRoomsProvider roomsProvider, Func<Services.ISession> sessionProvider)
        {
            _context = context;
            _roomsProvider = roomsProvider;
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();
                await session.OpenAsync(sessionId);

                var tileAssignments = await _context.TileAssignments.ToListAsync();                

                var inRangeRooms = await _roomsProvider.AccumulateAllRoomsAsync(from, to);

                var newTilesForSession = inRangeRooms.ExcludeBySession(session);
                var tiles = ComposeWithAssignment(newTilesForSession, tileAssignments).ToList();

                await session.CloseAsync();
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

        private IEnumerable<Tile> ComposeWithAssignment(IEnumerable<FlattenedRoom> rooms, IEnumerable<TileAssignment> tileAssignments)
        {
            var random = new Random();

            var tiles = rooms
                .GroupJoin(
                    tileAssignments,
                    room => room.Id,
                    assignment => assignment.Id,
                    (room, assignments) => new { room, assignments }
                )
                .SelectMany(
                    x => x.assignments.DefaultIfEmpty(),
                    (join, assignment) => new { join.room, assignment }
                );

            foreach (var tile in tiles)
            {
                string color;
                long? roomId;
                if (tile.assignment == null)
                {
                    color = $"booking{(random.Next() % 8) + 1}";
                    roomId = null;
                    _context.TileAssignments.Add(new TileAssignment(tile.room.Id, color));
                } else
                {
                    color = tile.assignment.Color;
                    roomId = tile.assignment.RoomId;
                }

                yield return new Tile(
                    id:             tile.room.Id,
                    bookingId:      tile.room.Booking.BookingNumber.ToString(),
                    name:           $"{tile.room.Booking.FirstName} {tile.room.Booking.LastName}",
                    lastModified:   tile.room.Booking.LastModified,
                    from:           tile.room.From.ToString("yyyy-MM-dd"),
                    nights:         Convert.ToUInt32((tile.room.To - tile.room.From).Days),
                    roomType:       tile.room.Room.RoomName,
                    entity:         tile.room.Room.RoomName,
                    persons:        Convert.ToUInt32(tile.room.Room.Guests.Count()),
                    color:          color
                )
                {
                    Status = tile.room.Booking.Status,
                    RoomId = roomId
                };
            }
        }
    }
}
