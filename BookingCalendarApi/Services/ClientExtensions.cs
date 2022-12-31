using BookingCalendarApi.Models;
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
                        .ToList()
                });
        }

        public static IEnumerable<Reservation> SelectByQuery(this IEnumerable<Reservation> reservations, string query)
        {
            return reservations
                .Select(reservation => new Reservation(reservation.ReservationId)
                {
                    Guests = reservation.Guests
                        .Where(guest =>
                            guest.FirstName != string.Empty
                            && $"{guest.FirstName} {guest.LastName}".Contains(query, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                });
        }

        public static IEnumerable<Client> ComposeResponse(this IEnumerable<Reservation> reservations)
        {
            return reservations
                .SelectMany(
                    x => x.Guests,
                    (reservation, guest) => new Client(
                        id:             guest.GuestId,
                        bookingId:      reservation.ReservationId.ToString(),
                        name:           guest.FirstName,
                        surname:        guest.LastName,
                        dateOfBirth:    guest.BirthDate.Trim() != "" ? DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd") : ""
                    )
                    {
                        StateOfBirth = guest.BirthCountry,
                        PlaceOfBirth = guest.BirthCity,
                        ProvinceOfBirth = guest.BirthCounty
                    }
                );
        }
    }
}
