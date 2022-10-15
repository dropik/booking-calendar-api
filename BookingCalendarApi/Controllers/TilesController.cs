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
                var guid = sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();

                var sessions = await _context.Sessions.Where(session => session.Id.Equals(guid)).ToListAsync();
                var tileAssignments = await _context.TileAssignments.ToListAsync();                

                var inRangeRooms = await AccumulateAllRoomsAsync(from, to);

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

        private async Task<IEnumerable<FlattenedRoom>> AccumulateAllRoomsAsync(string from, string to)
        {
            var (fromDate, toDate) = GetInitialRange(from, to);
            var inRangeRooms = await GetFlattenedRoomsAsync(fromDate, toDate);

            // extending search range to fetch all tiles that might possibly collide with
            // tiles that fall in the range, to ensure correct collision detection in front-end
            if (inRangeRooms.Any())
            {
                (fromDate, toDate) = GetExtendedRange(inRangeRooms);
                inRangeRooms = await GetFlattenedRoomsAsync(fromDate, toDate);
            }

            return inRangeRooms;
        }

        private (DateTime fromDate, DateTime toDate) GetInitialRange(string from, string to)
        {
            return
            (
                DateTime.ParseExact(from, "yyyy-MM-dd", null),
                DateTime.ParseExact(to, "yyyy-MM-dd", null)
            );
        }

        private (DateTime fromDate, DateTime toDate) GetExtendedRange(IEnumerable<FlattenedRoom> rooms)
        {
            var firstArrival = rooms.OrderBy(tile => tile.From).First();
            var lastDeparture = rooms.OrderBy(tile => tile.To).Last();

            return
            (
                firstArrival.From,
                lastDeparture.To.AddDays(-1)
            );
        }

        private async Task<IEnumerable<FlattenedRoom>> GetFlattenedRoomsAsync(DateTime fromDate, DateTime toDate)
        {
            var arrivalFrom = fromDate.AddDays(-30).ToString("yyyyMMdd");
            var arrivalTo = toDate.ToString("yyyyMMdd");

            var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);

            return bookings
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => new FlattenedRoom(
                        room.StayId.ToString(),
                        DateTime.ParseExact(room.Arrival, "yyyyMMdd", null),
                        DateTime.ParseExact(room.Departure, "yyyyMMdd", null),
                        booking,
                        room
                    ))
                .Where(roomData => (roomData.To - fromDate).Days >= 0);
        }
    }

    class FlattenedRoom
    {
        public FlattenedRoom(
            string id,
            DateTime from,
            DateTime to,
            Models.Iperbooking.Bookings.Booking booking,
            Models.Iperbooking.Bookings.Room room
        )
        {
            Id = id;
            From = from;
            To = to;
            Booking = booking;
            Room = room;
        }

        public string Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Models.Iperbooking.Bookings.Booking Booking { get; set; }
        public Models.Iperbooking.Bookings.Room Room { get; set; }
    }
}
