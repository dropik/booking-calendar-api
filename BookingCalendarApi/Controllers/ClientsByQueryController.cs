using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/clients-by-query")]
    [ApiController]
    public class ClientsByQueryController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;

        public ClientsByQueryController(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientWithBooking>>> GetAsync(string query, string from, string to)
        {
            try
            {
                var arrivalFrom = DateTime.ParseExact(from, "yyyy-MM-dd", null).ToString("yyyyMMdd");
                var arrivalTo = DateTime.ParseExact(to, "yyyy-MM-dd", null).ToString("yyyyMMdd");
                var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);

                string bookingIds = "";
                foreach (var booking in bookings)
                {
                    bookingIds += $"{booking.BookingNumber},";
                }

                var guestsResponse = await _iperbooking.GetGuestsAsync(bookingIds);

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
                                dateOfBirth: DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
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
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
