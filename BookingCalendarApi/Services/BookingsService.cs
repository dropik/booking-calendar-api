using BookingCalendarApi.Exceptions;
using BookingCalendarApi.Models.Entities;
using BookingCalendarApi.Models.Entities.EntityContents;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System.Formats.Cbor;

namespace BookingCalendarApi.Services
{
    public class BookingsService : IBookingsService
    {
        private readonly IIperbooking _iperbooking;
        private readonly BookingCalendarContext _context;

        private List<Booking> Bookings { get; set; } = new();
        private GuestsResponse GuestsResponse { get; set; } = new();
        private Guid SessionId { get; set; }
        private List<SessionBooking> SessionBookings { get; set; } = new();
        private SessionEntry? Entry { get; set; }

        public BookingsService(IIperbooking iperbooking, BookingCalendarContext context)
        {
            _iperbooking = iperbooking;
            _context = context;
        }

        public async Task<BookingResponse<List<ClientResponse>>> Get(string id, string from)
        {
            var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
            var arrivalFrom = fromDate.AddDays(-15).ToString("yyyy-MM-dd");
            var arrivalTo = fromDate.AddDays(15).ToString("yyyy-MM-dd");
            await Task.WhenAll(
                FetchBookings(arrivalFrom, arrivalTo, exactPeriod: true),
                FetchGuests(id)
            );
            var booking = Bookings.SelectById(id);

            var query = (from b in booking
                         join color in _context.ColorAssignments on b.BookingNumber.ToString() equals color.BookingId into gj
                         from color in gj.DefaultIfEmpty()
                         select new { Booking = b, Color = color }
                        ).ToList()
                        .Select(join => new BookingResponse<List<ClientResponse>>(
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
                                    .Select(join => new TileResponse<List<ClientResponse>>(
                                        id: $"{join.Room.StayId}-{join.Room.Arrival}-{join.Room.Departure}",
                                        from: DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                        nights: Convert.ToUInt32((DateTime.ParseExact(join.Room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null)).Days),
                                        roomType: join.Room.RoomName,
                                        rateId: join.Room.RateId,
                                        persons: join.Reservation.Guests
                                            .Where(guest => guest.ReservationRoomId == join.Room.StayId)
                                            .Select(guest => new ClientResponse(
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
                                        RoomId = join.Assignment?.RoomId
                                    })
                                    .ToList()
                        });

            if (!query.Any())
            {
                throw new BookingCalendarException(BCError.NOT_FOUND, "No booking found.");
            }

            return query.First();
        }

        public async Task<List<ShortBookingResponse>> GetByName(string from, string to, string? name)
        {
            var definedName = name ?? "";
            var bookingsByName = (await _iperbooking.GetBookings(from, to))
                .SelectInRange(from, to)
                .SelectByName(definedName);

            var bookingsWithColors = from booking in bookingsByName
                                     join color in _context.ColorAssignments on booking.BookingNumber.ToString() equals color.BookingId into gj
                                     from color in gj.DefaultIfEmpty()
                                     select new { Booking = booking, Color = color };

            return bookingsWithColors
                .Select(join => new ShortBookingResponse(
                    id: join.Booking.BookingNumber.ToString(),
                    name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                    lastModified: join.Booking.LastModified,
                    from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    occupations: Convert.ToUInt32(join.Booking.Rooms.Count)
                )
                {
                    Status = join.Booking.Status,
                    Color = join.Color?.Color
                })
                .ToList();
        }

        public async Task<BookingsBySessionResponse> GetBySession(string from, string to, string? sessionId)
        {
            await Task.WhenAll(
                    OpenSessionWithId(sessionId),
                    FetchBookings(from, to)
                );

            var bookings = Bookings
                .SelectInRange(from, to, true)
                .Where(booking => !SessionBookings
                    .Where(sessionBooking => sessionBooking.Equals(new SessionBooking(booking.BookingNumber.ToString(), booking.LastModified)))
                    .Any());

            var bookingsWithColors = from booking in bookings
                                     join color in _context.ColorAssignments on booking.BookingNumber.ToString() equals color.BookingId into gj
                                     from color in gj.DefaultIfEmpty()
                                     select new { Booking = booking, Color = color };

            var bookingsWithGuestsCount = bookingsWithColors.Select(join => new BookingResponse<uint>(
                    id: join.Booking.BookingNumber.ToString(),
                    name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                    lastModified: join.Booking.LastModified,
                    from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"))
            {
                Status = join.Booking.Status,
                Color = join.Color?.Color,
                Tiles = (from room in @join.Booking.Rooms
                         join assignment in _context.RoomAssignments on $"{room.StayId}-{room.Arrival}-{room.Departure}" equals assignment.Id into gj
                         from assignment in gj.DefaultIfEmpty()
                         select new { Room = room, Assignment = assignment })
                                 .ToList()
                                 .Select(join => new TileResponse<uint>(
                                    id: $"{join.Room.StayId}-{join.Room.Arrival}-{join.Room.Departure}",
                                    from: DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                    nights: Convert.ToUInt32((DateTime.ParseExact(join.Room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null)).Days),
                                    roomType: join.Room.RoomName,
                                    rateId: join.Room.RateId,
                                    persons: Convert.ToUInt32(join.Room.Guests.Count))
                                 {
                                     RoomId = join.Assignment?.RoomId
                                 })
                                 .ToList()
            })
                .ToList();

            return new BookingsBySessionResponse(SessionId.ToString())
            {
                Bookings = bookingsWithGuestsCount
            };
        }

        public async Task Ack(AckBookingsRequest request)
        {
            var bookings = request.Bookings;
            var sessionId = request.SessionId;

            if (bookings == null)
            {
                return;
            }

            await OpenSessionWithId(sessionId);
            AddBookingsToSession(bookings);

            await _context.SaveChangesAsync();
        }

        private async Task OpenSessionWithId(string? sessionId)
        {
            SessionId = GetSessionId(sessionId);
            SessionBookings = new();
            Entry = await _context.Sessions.SingleOrDefaultAsync(session => session.Id.Equals(SessionId));
            if (Entry != null && !Entry.Id.Equals(Guid.Empty))
            {
                var data = Entry.Data;
                var reader = new CborReader(data);
                reader.ReadStartArray();
                while (reader.PeekState() != CborReaderState.EndArray)
                {
                    var bookingId = reader.ReadTextString();
                    var lastModified = reader.ReadTextString();
                    SessionBookings.Add(new SessionBooking(bookingId, lastModified));
                }
            }
        }

        private static Guid GetSessionId(string? sessionId)
        {
            try
            {
                return sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();
            }
            catch (Exception)
            {
                return Guid.NewGuid();
            }
        }

        private void AddBookingsToSession(IEnumerable<SessionBooking> bookings)
        {
            foreach (var booking in bookings)
            {
                if (!SessionBookings.Contains(booking))
                {
                    SessionBookings.Add(booking);
                }
            }

            var writer = new CborWriter();
            writer.WriteStartArray(SessionBookings.Count * 2);
            foreach (var booking in SessionBookings)
            {
                writer.WriteTextString(booking.BookingId);
                writer.WriteTextString(booking.LastModified);
            }
            writer.WriteEndArray();

            var data = writer.Encode();

            if (Entry != null)
            {
                Entry.Data = data;
            }
            else
            {
                _context.Sessions.Add(new() { Id = SessionId, Data = data });
            }
        }

        private async Task FetchBookings(string from, string to, bool exactPeriod = false)
        {
            Bookings = await _iperbooking.GetBookings(from, to, exactPeriod);
        }

        private async Task FetchGuests(string id)
        {
            GuestsResponse = await _iperbooking.GetGuests(id);
        }
    }
}
