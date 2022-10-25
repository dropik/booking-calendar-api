using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class TileComposer : ITileComposer
    {
        private readonly BookingCalendarContext _context;

        public TileComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<Tile<uint>> Compose(IEnumerable<Room<Guest>> rooms)
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
                    (join, assignment) => new Tile<uint>(
                        id: $"{join.room.StayId}-{join.room.Arrival}-{join.room.Departure}",
                        from: DateTime.ParseExact(join.room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        nights: Convert.ToUInt32((DateTime.ParseExact(join.room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.room.Arrival, "yyyyMMdd", null)).Days),
                        roomType: join.room.RoomName,
                        persons: Convert.ToUInt32(join.room.Guests.Count())
                    )
                    {
                        RoomId = assignment?.RoomId ?? null
                    }
                );
        }
    }
}
