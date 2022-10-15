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
                var random = new Random();

                var session = _sessionProvider();
                await session.OpenAsync(sessionId);

                var tileAssignments = await _context.TileAssignments.ToListAsync();                

                var inRangeRooms = await _roomsProvider.AccumulateAllRoomsAsync(from, to);

                var tiles = inRangeRooms
                    .ExcludeBySession(session)
                    .GroupJoin(
                        tileAssignments,
                        roomData => roomData.Id,
                        assignment => assignment.Id,
                        (roomData, assignments) => new { roomData, assignments }
                    )
                    .SelectMany(
                        x => x.assignments.DefaultIfEmpty(),
                        (join, assignment) => new Tile(
                            id:             join.roomData.Id,
                            bookingId:      join.roomData.Booking.BookingNumber.ToString(),
                            name:           $"{join.roomData.Booking.FirstName} {join.roomData.Booking.LastName}",
                            lastModified:   join.roomData.Booking.LastModified,
                            from:           join.roomData.From.ToString("yyyy-MM-dd"),
                            nights:         Convert.ToUInt32((join.roomData.To - join.roomData.From).Days),
                            roomType:       join.roomData.Room.RoomName,
                            entity:         join.roomData.Room.RoomName,
                            persons:        Convert.ToUInt32(join.roomData.Room.Guests.Count()),
                            color:          assignment?.Color ?? $"booking{(random.Next() % 8) + 1}"
                        )
                        {
                            Status = join.roomData.Booking.Status,
                            RoomId = assignment?.RoomId ?? null
                        }
                    )
                    .ToList();

                foreach (var tile in tiles)
                {
                    if (!tileAssignments.Any(a => a.Id.Equals(tile.Id)))
                    {
                        _context.TileAssignments.Add(new TileAssignment(tile.Id, tile.Color) { RoomId = tile.RoomId });
                    }
                }

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
    }
}
