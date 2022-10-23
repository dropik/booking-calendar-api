using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class BookingWithGuestsExtensions
    {
        public static IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> ExcludeByEmptyGuestData(this IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> bookings) =>
            bookings
            .Select(booking => new AssignedBooking<Models.Iperbooking.Guests.Guest>(booking.Booking)
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
                        Guests = room.Guests
                            .Where(guest => guest.FirstName != "")
                    })
                    .Where(room => room.Guests.Any())
            })
            .Where(booking => booking.Rooms.Any());
    }
}
