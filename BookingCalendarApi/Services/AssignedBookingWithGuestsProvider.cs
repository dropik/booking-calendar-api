using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class AssignedBookingWithGuestsProvider : IAssignedBookingWithGuestsProvider
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly BookingCalendarContext _context;
        private readonly Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer> _assignedBookingComposerProvider;
        private readonly IIperbooking _iperbooking;

        public AssignedBookingWithGuestsProvider(
            IBookingsProvider bookingsProvider,
            BookingCalendarContext context,
            Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer> assignedBookingComposerProvider,
            IIperbooking iperbooking)
        {
            _bookingsProvider = bookingsProvider;
            _context = context;
            _assignedBookingComposerProvider = assignedBookingComposerProvider;
            _iperbooking = iperbooking;
        }

        public IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> Bookings { get; private set; } = new List<AssignedBooking<Models.Iperbooking.Guests.Guest>>();

        public async Task FetchAsync(string from, string? to = null, bool exactPeriod = true)
        {
            to ??= DateTime.ParseExact(from, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");

            await _bookingsProvider.FetchBookingsAsync(from, to, exactPeriod);
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

            Bookings = assignedBookings
                .Join(
                    guestResponse.Reservations,
                    booking => booking.Booking.BookingNumber,
                    reservation => reservation.ReservationId,
                    (booking, reservation) => new AssignedBooking<Models.Iperbooking.Guests.Guest>(booking.Booking)
                    {
                        Rooms = booking.Rooms
                            .Select(
                                room => new AssignedRoom<Models.Iperbooking.Guests.Guest>(
                                stayId: room.StayId,
                                roomName: room.RoomName,
                                arrival: room.Arrival,
                                departure: room.Departure
                            )
                                {
                                    RoomId = room.RoomId,
                                    Guests = reservation.Guests
                                    .Where(guest => guest.ReservationRoomId == room.StayId)
                                    .Where(guest => guest.FirstName != "")
                                })
                            .Where(room => room.Guests.Any())
                    }
                )
                .Where(booking => booking.Rooms.Any());
        }
    }
}
