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

        public TilesController(BookingCalendarContext context, IRoomsProvider roomsProvider)
        {
            _context = context;
            _roomsProvider = roomsProvider;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var random = new Random();
                var guid = sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();

                var sessions = await _context.Sessions.Where(session => session.Id.Equals(guid)).ToListAsync();
                var tileAssignments = await _context.TileAssignments.ToListAsync();                

                var inRangeRooms = await _roomsProvider.AccumulateAllRoomsAsync(from, to);

                var tiles = inRangeRooms
                    .Where(roomData => !sessions.Any(session => session.Equals(new Session(guid, roomData.Id, roomData.Booking.LastModified))))
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
                    var newSession = new Session(guid, tile.Id, tile.LastModified);
                    if (!sessions.Any(session => session.Equals(newSession)))
                    {
                        _context.Sessions.Add(newSession);
                    } else
                    {
                        _context.Entry(newSession).State = EntityState.Modified;
                    }

                    if (!tileAssignments.Any(a => a.Id.Equals(tile.Id)))
                    {
                        _context.TileAssignments.Add(new TileAssignment(tile.Id, tile.Color) { RoomId = tile.RoomId });
                    }
                }

                await _context.SaveChangesAsync();

                return new TileResponse(guid.ToString())
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
