using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class BookingWithGuestsProvider : IBookingWithGuestsProvider
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly BookingCalendarContext _context;
        private readonly Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer> _assignedBookingComposerProvider;
        private readonly IIperbooking _iperbooking;
        private readonly Func<IEnumerable<Reservation>, IBookingWithGuestsComposer> _bookingWithGuestsComposerProvider;

        public BookingWithGuestsProvider(
            IBookingsProvider bookingsProvider,
            BookingCalendarContext context,
            Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer> assignedBookingComposerProvider,
            IIperbooking iperbooking,
            Func<IEnumerable<Reservation>, IBookingWithGuestsComposer> bookingWithGuestsComposerProvider)
        {
            _bookingsProvider = bookingsProvider;
            _context = context;
            _assignedBookingComposerProvider = assignedBookingComposerProvider;
            _iperbooking = iperbooking;
            _bookingWithGuestsComposerProvider = bookingWithGuestsComposerProvider;
        }

        public IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> Bookings { get; private set; } = new List<AssignedBooking<Models.Iperbooking.Guests.Guest>>();

        public async Task FetchAsync(string date)
        {
            var to = DateTime.ParseExact(date, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");

            await _bookingsProvider.FetchBookingsAsync(date, to, exactPeriod: true);
            var bookings = _bookingsProvider.Bookings
                .ExcludeCancelled();

            var stayIds = bookings
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => $"{room.StayId}-{room.Arrival}-{room.Departure}"
            );

            var assignments = await _context.RoomAssignments
                .Where(assignment => stayIds.Contains(assignment.Id))
                .ToListAsync();

            var assignedBookingComposer = _assignedBookingComposerProvider(assignments);

            var assignedBookings = bookings
                .UseComposer(assignedBookingComposer)
                .ExcludeNotAssigned();

            var bookingIds = "";
            foreach (var booking in assignedBookings)
            {
                bookingIds += $"{booking.Booking.BookingNumber},";
            }

            var guestResponse = await _iperbooking.GetGuestsAsync(bookingIds);
            var bookingWithGuestsComposer = _bookingWithGuestsComposerProvider(guestResponse.Reservations);
            Bookings = assignedBookings.UseComposer(bookingWithGuestsComposer);
        }
    }
}
