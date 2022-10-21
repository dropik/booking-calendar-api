using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class StayComposer : IStayComposer
    {
        public IEnumerable<Stay> Compose(IEnumerable<Booking> source)
        {
            return source
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => new Stay(
                        stayId:         room.StayId,
                        bookingNumber:  booking.BookingNumber,
                        arrival:        room.Arrival,
                        departure:      room.Departure
                    )
                    {
                        Guests = room.Guests
                    }
                );
        }
    }
}
