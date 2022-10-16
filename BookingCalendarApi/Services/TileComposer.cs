using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class TileComposer : ITileComposer
    {
        private readonly BookingCalendarContext _context;
        private IEnumerable<TileAssignment> TileAssignments { get; set; } = new List<TileAssignment>();

        public TileComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public async Task OpenAsync()
        {
            TileAssignments = await _context.TileAssignments.ToListAsync();
        }

        public IEnumerable<Tile> Compose(IEnumerable<FlattenedRoom> rooms)
        {
            var random = new Random();

            var tiles = rooms
                .GroupJoin(
                    TileAssignments,
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
                }
                else
                {
                    color = tile.assignment.Color;
                    if (tile.room.Booking.Status == Models.Iperbooking.Bookings.Status.Cancelled)
                    {
                        roomId = null;
                        tile.assignment.RoomId = null;
                    }
                    else
                    {
                        roomId = tile.assignment.RoomId;
                    }
                }

                yield return new Tile(
                    id: tile.room.Id,
                    bookingId: tile.room.Booking.BookingNumber.ToString(),
                    name: $"{tile.room.Booking.FirstName} {tile.room.Booking.LastName}",
                    lastModified: tile.room.Booking.LastModified,
                    from: tile.room.From.ToString("yyyy-MM-dd"),
                    nights: Convert.ToUInt32((tile.room.To - tile.room.From).Days),
                    roomType: tile.room.Room.RoomName,
                    entity: tile.room.Room.RoomName,
                    persons: Convert.ToUInt32(tile.room.Room.Guests.Count()),
                    color: color
                )
                {
                    Status = tile.room.Booking.Status,
                    RoomId = roomId
                };
            }
        }
    }
}
