using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class BookingWithGuestsComposer : IBookingWithGuestsComposer
    {
        private readonly IEnumerable<Models.Iperbooking.Guests.Reservation> _reservations;

        public BookingWithGuestsComposer(IEnumerable<Models.Iperbooking.Guests.Reservation> reservations)
        {
            _reservations = reservations;
        }

        public IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> Compose(IEnumerable<AssignedBooking<Guest>> source) =>
            source
            .Join(_reservations,
                booking => booking.Booking.BookingNumber,
                reservation => reservation.ReservationId,
                (booking, reservation) => new AssignedBooking<Models.Iperbooking.Guests.Guest>(booking.Booking)
                {
                    Rooms = booking.Rooms
                        .Select(room => new AssignedRoom<Models.Iperbooking.Guests.Guest>(
                                stayId: room.StayId,
                                roomName: room.RoomName,
                                arrival: room.Arrival,
                                departure: room.Departure
                            )
                        {
                            RoomId = room.RoomId,
                            Guests = reservation.Guests
                                .Where(guest => guest.ReservationRoomId == room.StayId)
                        })
                });
    }
}
