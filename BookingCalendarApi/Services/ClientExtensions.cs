using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public static class ClientExtensions
    {
        public static IEnumerable<Reservation> SelectByStayId(this IEnumerable<Reservation> reservations, string stayId)
        {
            return reservations
                .Select(reservation => new Reservation(reservation.ReservationId)
                {
                    Guests = reservation.Guests
                        .Where(guest => guest.ReservationRoomId.ToString() == stayId)
                });
        }

        public static IEnumerable<ResponseClient> ComposeResponse(this IEnumerable<Reservation> reservations)
        {
            return reservations
                .SelectMany(
                    x => x.Guests,
                    (reservation, guest) => new ResponseClient(
                        id:             guest.GuestId,
                        bookingId:      reservation.ReservationId.ToString(),
                        name:           guest.FirstName,
                        surname:        guest.LastName,
                        dateOfBirth:    guest.BirthDate
                    )
                    {
                        StateOfBirth = guest.BirthCountry,
                        PlaceOfBirth = guest.BirthCity
                    }
                );
        }
    }
}
