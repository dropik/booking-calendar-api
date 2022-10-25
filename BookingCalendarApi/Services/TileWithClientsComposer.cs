using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public class TileWithClientsComposer : ITileWithClientsComposer
    {
        private readonly BookingCalendarContext _context;
        private readonly Reservation _reservation;

        public TileWithClientsComposer(BookingCalendarContext context, Reservation reservation)
        {
            _context = context;
            _reservation = reservation;
        }

        public IEnumerable<Tile<IEnumerable<Client>>> Compose(IEnumerable<Room<Models.Iperbooking.Bookings.Guest>> rooms)
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
                    (join, assignment) => new Tile<IEnumerable<Client>>(
                        id: $"{join.room.StayId}-{join.room.Arrival}-{join.room.Departure}",
                        from: DateTime.ParseExact(join.room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        nights: Convert.ToUInt32((DateTime.ParseExact(join.room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.room.Arrival, "yyyyMMdd", null)).Days),
                        roomType: join.room.RoomName,
                        persons: _reservation.Guests
                            .Where(guest => guest.ReservationRoomId == join.room.StayId)
                            .Select(guest => new Client(
                                id: guest.GuestId,
                                bookingId: _reservation.ReservationId.ToString(),
                                name: guest.FirstName,
                                surname: guest.LastName,
                                dateOfBirth: guest.BirthDate.Trim() != string.Empty ? DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd") : ""
                            )
                            {
                                PlaceOfBirth = guest.BirthCity,
                                ProvinceOfBirth = guest.BirthCounty,
                                StateOfBirth = guest.BirthCountry
                            }
                        )
                    )
                    {
                        RoomId = assignment?.RoomId ?? null
                    }
                );
        }
    }
}
