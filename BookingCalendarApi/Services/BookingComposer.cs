using BookingCalendarApi.Controllers.Internal;
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

        public IEnumerable<ResponseBooking> Compose(IEnumerable<Booking> bookings)
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
                    (join, assignment) => new ResponseBooking(
                        id: join.booking.BookingNumber.ToString(),
                        name: $"{join.booking.FirstName} {join.booking.LastName}",
                        lastModified: join.booking.LastModified
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