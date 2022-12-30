using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class AssignedBookingWithGuestsProvider : IAssignedBookingWithGuestsProvider
    {
        private readonly BookingCalendarContext _context;
        private readonly IAssignedBookingComposer _assignedBookingComposer;
        private readonly IIperbooking _iperbooking;
        private readonly DataContext _dataContext;

        public AssignedBookingWithGuestsProvider(
            BookingCalendarContext context,
            IAssignedBookingComposer assignedBookingComposer,
            IIperbooking iperbooking,
            DataContext dataContext)
        {
            _context = context;
            _assignedBookingComposer = assignedBookingComposer;
            _iperbooking = iperbooking;
            _dataContext = dataContext;
        }

        public async Task<List<AssignedBooking<Guest>>> Get(string from, string? to = null, bool exactPeriod = true)
        {
            to ??= DateTime.ParseExact(from, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");

            var bookings = (await _iperbooking.GetBookingsAsync(from, to, exactPeriod))
                .ExcludeCancelled();

            var stayIds = bookings
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => $"{room.StayId}-{room.Arrival}-{room.Departure}"
            );

            _dataContext.RoomAssignments.AddRange(await _context.RoomAssignments
                .Where(assignment => stayIds.Contains(assignment.Id))
                .ToListAsync());

            var assignedBookings = bookings
                .UseComposer(_assignedBookingComposer)
                .ExcludeNotAssigned();

            var bookingIds = "";
            foreach (var booking in assignedBookings)
            {
                bookingIds += $"{booking.Booking.BookingNumber},";
            }

            var guestResponse = await _iperbooking.GetGuestsAsync(bookingIds);

            return assignedBookings
                .Join(
                    guestResponse.Reservations,
                    booking => booking.Booking.BookingNumber,
                    reservation => reservation.ReservationId,
                    (booking, reservation) => new AssignedBooking<Guest>(booking.Booking)
                    {
                        Rooms = booking.Rooms
                            .Select(
                                room => new AssignedRoom<Guest>(
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
                .Where(booking => booking.Rooms.Any())
                .ToList();
        }
    }
}
