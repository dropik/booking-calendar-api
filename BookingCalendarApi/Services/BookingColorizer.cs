using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class BookingColorizer : IBookingColorizer
    {
        private readonly BookingCalendarContext _context;

        public BookingColorizer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<ColorizedBooking> Colorize(IEnumerable<Booking> bookings)
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
                    (join, assignment) => new ColorizedBooking(join.booking, assignment?.Color)
                );
        }
    }
}