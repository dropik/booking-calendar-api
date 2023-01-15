using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class ClientsService : IClientsService
    {
        private readonly IIperbooking _iperbooking;

        public ClientsService(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }

        public async Task<List<ClientWithBooking>> GetByQuery(string query, string from, string to)
        {
            var bookings = await _iperbooking.GetBookings(from, to, exactPeriod: true);

            string bookingIds = "";
            foreach (var booking in bookings)
            {
                bookingIds += $"{booking.BookingNumber},";
            }

            var guestsResponse = await _iperbooking.GetGuests(bookingIds);

            return guestsResponse.Reservations
                .SelectByQuery(query)
                .Join(
                    bookings,
                    reservation => reservation.ReservationId,
                    booking => booking.BookingNumber,
                    (reservation, booking) => reservation.Guests
                        .Select(guest => new ClientWithBooking(
                            id: guest.GuestId,
                            bookingId: reservation.ReservationId.ToString(),
                            name: guest.FirstName,
                            surname: guest.LastName,
                            dateOfBirth:
                                guest.BirthDate.Length == 0
                                ? ""
                                : DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                            bookingName: $"{booking.FirstName} {booking.LastName}",
                            bookingFrom: DateTime.ParseExact(booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                            bookingTo: DateTime.ParseExact(booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd")
                        )
                        {
                            PlaceOfBirth = guest.BirthCity,
                            ProvinceOfBirth = guest.BirthCounty,
                            StateOfBirth = guest.BirthCountry
                        })
                )
                .SelectMany(guests => guests)
                .ToList();
        }

        public async Task<List<Client>> GetByTile(string bookingId, string tileId)
        {
            var stayId = tileId.Split("-")[0];
            var response = await _iperbooking.GetGuests(bookingId);

            return response.Reservations
                .SelectByStayId(stayId)
                .ComposeResponse()
                .ToList();
        }
    }
}
