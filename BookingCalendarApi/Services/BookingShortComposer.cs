using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class BookingShortComposer : IBookingShortComposer
    {
        private readonly BookingCalendarContext _context;

        public BookingShortComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<ShortBooking> Compose(IEnumerable<Booking> bookings)
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
                    (join, assignment) => new ShortBooking(
                        id:             join.booking.BookingNumber.ToString(),
                        name:           $"{join.booking.FirstName} {join.booking.LastName}",
                        lastModified:   join.booking.LastModified,
                        from:           DateTime.ParseExact(join.booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        to:             DateTime.ParseExact(join.booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        occupations:    Convert.ToUInt32(join.booking.Rooms.Count)
                    )
                    {
                        Status = join.booking.Status,
                        Color = assignment?.Color
                    }
                );
        }
    }
}
