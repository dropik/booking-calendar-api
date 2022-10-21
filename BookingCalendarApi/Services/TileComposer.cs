using BookingCalendarApi.Models;
using Room = BookingCalendarApi.Models.Iperbooking.Bookings.Room;

namespace BookingCalendarApi.Services
{
    public class TileComposer : ITileComposer
    {
        private readonly BookingCalendarContext _context;

        public TileComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<Tile> Compose(IEnumerable<Room> rooms)
        {
            return rooms
                .GroupJoin(
                    _context.RoomAssignments,
                    room => $"{room.StayId}-{room.Arrival}-{room.Departure}",
                    assignment => assignment.Id,
                    (room, assignments) => new { room, assignments }
                )
                .SelectMany(
                    x => x.assignments.DefaultIfEmpty(),
                    (join, assignment) => new Tile(
                        id: $"{join.room.StayId}-{join.room.Arrival}-{join.room.Departure}",
                        from: DateTime.ParseExact(join.room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        nights: Convert.ToUInt32((DateTime.ParseExact(join.room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.room.Arrival, "yyyyMMdd", null)).Days),
                        roomType: join.room.RoomName,
                        entity: join.room.RoomName,
                        persons: Convert.ToUInt32(join.room.Guests.Count)
                    )
                    {
                        RoomId = assignment?.RoomId ?? null
                    }
                );
        }
    }
}
