using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class BookingComposer : IBookingComposer
    {
        private readonly BookingCalendarContext _context;
        private readonly ITileComposer _tileComposer;

        public BookingComposer(BookingCalendarContext context, ITileComposer tileComposer)
        {
            _context = context;
            _tileComposer = tileComposer;
        }

        public IEnumerable<Booking<uint>> Compose(IEnumerable<Booking> bookings)
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
                    (join, assignment) => new Booking<uint>(
                        id:             join.booking.BookingNumber.ToString(),
                        name:           $"{join.booking.FirstName} {join.booking.LastName}",
                        lastModified:   join.booking.LastModified,
                        from: DateTime.ParseExact(join.booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        to: DateTime.ParseExact(join.booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd")
                    )
                    {
                        Status = join.booking.Status,
                        Color = assignment?.Color,
                        Tiles = join.booking.Rooms.UseComposer(_tileComposer)
                    }
                );
        }
    }
}