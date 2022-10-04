using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;
        private readonly BookingCalendarContext _context;

        public TilesController(IIperbooking iperbooking, BookingCalendarContext context)
        {
            _iperbooking = iperbooking;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var random = new Random();

                var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
                var toDate = DateTime.ParseExact(to, "yyyy-MM-dd", null);

                var arrivalFromDate = fromDate.AddDays(-30);

                var arrivalFrom = arrivalFromDate.ToString("yyyyMMdd");
                var arrivalTo = toDate.ToString("yyyyMMdd");

                var guid = sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();

                var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);
                var tiles = bookings
                    .SelectMany(
                        booking => booking.Rooms,
                        (booking, room) => new {
                            booking,
                            room,
                            Id = room.StayId.ToString(),
                            From = DateTime.ParseExact(room.Arrival, "yyyyMMdd", null),
                            To = DateTime.ParseExact(room.Departure, "yyyyMMdd", null)
                        })
                    .Where(roomData => !_context.Sessions.Contains(new Session(guid, roomData.Id)))
                    .Where(roomData => (roomData.To - fromDate).Days >= 0)
                    .GroupJoin(
                        _context.TileAssignments,
                        roomData => roomData.Id,
                        assignment => assignment.Id,
                        (roomData, assignments) => new { roomData, assignments }
                    )
                    .SelectMany(
                        x => x.assignments,
                        (join, assignment) => new Tile(
                            id:         join.roomData.Id,
                            bookingId:  join.roomData.booking.BookingNumber.ToString(),
                            name:       $"{join.roomData.booking.FirstName} {join.roomData.booking.LastName}",
                            from:       join.roomData.From.ToString("yyyy-MM-dd"),
                            nights:     Convert.ToUInt32((join.roomData.To - join.roomData.From).Days),
                            roomType:   join.roomData.room.RoomName,
                            entity:     join.roomData.room.RoomName,
                            persons:    Convert.ToUInt32(join.roomData.room.Guests.Count()),
                            color:      assignment?.Color ?? $"booking{(random.Next() % 8) + 1}"
                        )
                        {
                            RoomId = assignment?.RoomId ?? null
                        }
                    )
                    .ToList();

                foreach (var tile in tiles)
                {
                    _context.Sessions.Add(new Session(guid, tile.Id));
                    if (!_context.TileAssignments.Any(a => a.Id.Equals(tile.Id)))
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