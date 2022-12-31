using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;
        private readonly BookingCalendarContext _context;

        private List<Booking> Bookings { get; set; } = new();
        private GuestsResponse GuestsResponse { get; set; } = new GuestsResponse();

        public BookingController(IIperbooking iperbooking, BookingCalendarContext context)
        {
            _iperbooking = iperbooking;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Booking<List<Client>>>> GetAsync(string id, string from)
        {
            try
            {
                var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
                var arrivalFrom = fromDate.AddDays(-15).ToString("yyyy-MM-dd");
                var arrivalTo = fromDate.AddDays(15).ToString("yyyy-MM-dd");

                await Task.WhenAll(
                    FetchBookings(arrivalFrom, arrivalTo),
                    FetchGuests(id)
                );

                var booking = Bookings.SelectById(id);

                var query = (from b in booking
                            join color in _context.ColorAssignments on b.BookingNumber.ToString() equals color.BookingId into gj
                            from color in gj.DefaultIfEmpty()
                            select new { Booking = b, Color = color }
                            ).ToList()
                            .Select(join => new Booking<List<Client>>(
                                id: join.Booking.BookingNumber.ToString(),
                                name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                                lastModified: join.Booking.LastModified,
                                from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"))
                            {
                                Status = join.Booking.Status,
                                Color = join.Color?.Color,
                                Tiles = (from room in @join.Booking.Rooms
                                        join reservation in GuestsResponse.Reservations on @join.Booking.BookingNumber equals reservation.ReservationId
                                        join assignment in _context.RoomAssignments on $"{room.StayId}-{room.Arrival}-{room.Departure}" equals assignment.Id into gj
                                        from assignment in gj.DefaultIfEmpty()
                                        select new { Room = room, Reservation = reservation, Assignment = assignment })
                                        .ToList()
                                        .Select(join => new Tile<List<Client>>(
                                            id: $"{join.Room.StayId}-{join.Room.Arrival}-{join.Room.Departure}",
                                            from: DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                            nights: Convert.ToUInt32((DateTime.ParseExact(join.Room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null)).Days),
                                            roomType: join.Room.RoomName,
                                            rateId: join.Room.RateId,
                                            persons: join.Reservation.Guests
                                                .Where(guest => guest.ReservationRoomId == join.Room.StayId)
                                                .Select(guest => new Client(
                                                    id: guest.GuestId,
                                                    bookingId: join.Reservation.ReservationId.ToString(),
                                                    name: guest.FirstName,
                                                    surname: guest.LastName,
                                                    dateOfBirth: guest.BirthDate.Trim() != string.Empty ? DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd") : ""
                                                )
                                                {
                                                    PlaceOfBirth = guest.BirthCity,
                                                    ProvinceOfBirth = guest.BirthCounty,
                                                    StateOfBirth = guest.BirthCountry
                                                })
                                                .ToList())
                                            {
                                                RoomId = join.Assignment?.RoomId ?? null
                                            })
                                        .ToList()
                            });

                if (!query.Any())
                {
                    return NotFound();
                }

                return query.First();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task FetchBookings(string from, string to)
        {
            Bookings = await _iperbooking.GetBookingsAsync(from, to, exactPeriod: true);
        }

        private async Task FetchGuests(string id)
        {
            GuestsResponse = await _iperbooking.GetGuestsAsync(id);
        }
    }
}