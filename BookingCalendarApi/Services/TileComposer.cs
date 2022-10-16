using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class TileComposer : ITileComposer
    {
        private readonly BookingCalendarContext _context;

        public TileComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<Tile> Compose(IEnumerable<FlattenedRoom> rooms)
        {
            return rooms
                .GroupJoin(
                    _context.RoomAssignments,
                    room => room.Id,
                    assignment => assignment.Id,
                    (room, assignments) => new { room, assignments }
                )
                .SelectMany(
                    x => x.assignments.DefaultIfEmpty(),
                    (join, assignment) => new Tile(
                        id: join.room.Id,
                        bookingId: join.room.Booking.BookingNumber.ToString(),
                        name: $"{join.room.Booking.FirstName} {join.room.Booking.LastName}",
                        lastModified: join.room.Booking.LastModified,
                        from: join.room.From.ToString("yyyy-MM-dd"),
                        nights: Convert.ToUInt32((join.room.To - join.room.From).Days),
                        roomType: join.room.Room.RoomName,
                        entity: join.room.Room.RoomName,
                        persons: Convert.ToUInt32(join.room.Room.Guests.Count())
                    )
                    {
                        Status = join.room.Booking.Status,
                        Color = join.room.Booking.Color,
                        RoomId = assignment?.RoomId ?? null
                    }
                );
        }
    }
}
