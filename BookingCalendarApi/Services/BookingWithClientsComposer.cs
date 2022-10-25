using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public class BookingWithClientsComposer : IBookingWithClientsComposer
    {
        private readonly Func<Reservation, ITileWithClientsComposer> _tileComposerProvider;
        private readonly IEnumerable<Reservation> _reservations;
        private readonly BookingCalendarContext _context;

        public BookingWithClientsComposer(Func<Reservation, ITileWithClientsComposer> tileComposerProvider, IEnumerable<Reservation> reservations, BookingCalendarContext context)
        {
            _tileComposerProvider = tileComposerProvider;
            _reservations = reservations;
            _context = context;
        }

        public IEnumerable<Booking<IEnumerable<Client>>> Compose(IEnumerable<Booking> bookings)
        {
            return bookings
                .GroupJoin(
                        _context.ColorAssignments,
                        booking => booking.BookingNumber.ToString(),
                        assignment => assignment.BookingId,
                        (booking, assignments) => new { booking, assignments }
                    )
                    .SelectMany(
                        x => x.assignments.DefaultIfEmpty(),
                        (join, assignment) => new Booking<IEnumerable<Client>>(
                            id: join.booking.BookingNumber.ToString(),
                            name: $"{join.booking.FirstName} {join.booking.LastName}",
                            lastModified: join.booking.LastModified,
                            from: DateTime.ParseExact(join.booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                            to: DateTime.ParseExact(join.booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd")
                        )
                        {
                            Status = join.booking.Status,
                            Color = assignment?.Color,
                            Tiles = join.booking.Rooms
                                .UseComposer(_tileComposerProvider(
                                    _reservations.SingleOrDefault(reservation => reservation.ReservationId == join.booking.BookingNumber)
                                    ?? throw new Exception("Provided reservation for merging booking with its guests not found")))
                        }
                    );
        }
    }
}
