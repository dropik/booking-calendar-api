using BookingCalendarApi.Models;
using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PoliceController : ControllerBase
    {
        private readonly IAlloggiatiServiceSession _session;
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IIperbooking _iperbooking;
        private readonly IAssignedBookingComposer _assignedBookingComposer;
        private readonly Func<IEnumerable<Reservation>, IBookingWithGuestsComposer> _bookingWithGuestsComposerProvider;
        private readonly BookingCalendarContext _context;
        private readonly IAlloggiatiTableReader _tableReader;
        private readonly Func<IEnumerable<Place>, IPlaceConverter> _placeConverterProvider;
        private readonly Func<IEnumerable<PoliceNationCode>, INationConverter> _nationConverterProvider;
        private readonly IAccomodatedTypeSolver _accomodatedTypeSolver;
        private readonly ITrackedRecordSerializer _trackedRecordSerializer;

        public PoliceController(
            IAlloggiatiServiceSession session,
            IBookingsProvider bookingsProvider,
            IIperbooking iperbooking,
            IAssignedBookingComposer assignedBookingsComposer,
            Func<IEnumerable<Reservation>, IBookingWithGuestsComposer> bookingWithGuestsComposerProvider,
            BookingCalendarContext context,
            IAlloggiatiTableReader tableReader,
            Func<IEnumerable<Place>, IPlaceConverter> placeConverterProvider,
            Func<IEnumerable<PoliceNationCode>, INationConverter> nationConverterProvider,
            IAccomodatedTypeSolver accomodatedTypeSolver,
            ITrackedRecordSerializer trackedRecordSerializer
        )
        {
            _session = session;
            _bookingsProvider = bookingsProvider;
            _iperbooking = iperbooking;
            _assignedBookingComposer = assignedBookingsComposer;
            _bookingWithGuestsComposerProvider = bookingWithGuestsComposerProvider;
            _context = context;
            _tableReader = tableReader;
            _placeConverterProvider = placeConverterProvider;
            _nationConverterProvider = nationConverterProvider;
            _accomodatedTypeSolver = accomodatedTypeSolver;
            _trackedRecordSerializer = trackedRecordSerializer;
        }

        [HttpGet("ricevuta")]
        public async Task<IActionResult> GetRicevutaAsync(string date)
        {
            try
            {
                await _session.OpenAsync();
                var pdf = await _session.GetRicevutaAsync(DateTime.ParseExact(date, "yyyy-MM-dd", null));
                return File(pdf, "application/pdf", $"polizia-ricevuta-{date}.pdf");
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestAsync(SendRequest request)
        {
            try
            {
                var to = DateTime.ParseExact(request.Date, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");

                await _bookingsProvider.FetchBookingsAsync(request.Date, to, exactPeriod: true);
                var bookings = _bookingsProvider.Bookings
                    .ExcludeCancelled();

                var assignedBookings = bookings
                    .UseComposer(_assignedBookingComposer)
                    .ExcludeNotAssigned();

                var bookingIds = "";
                foreach (var booking in assignedBookings)
                {
                    bookingIds += $"{booking.Booking.BookingNumber},";
                }

                var guestResponse = await _iperbooking.GetGuestsAsync(bookingIds);
                var bookingWithGuestsComposer = _bookingWithGuestsComposerProvider(guestResponse.Reservations);

                var policeNations = await _context.PoliceNations.ToListAsync();
                var nationConverter = _nationConverterProvider(policeNations);

                await _session.OpenAsync();
                var placesStr = await _session.GetTableAsync(AlloggiatiService.TipoTabella.Luoghi);
                var places = _tableReader.ReadAsPlaces(placesStr);
                var placeConverter = _placeConverterProvider(places);

                var bookingsWithGuests = assignedBookings.UseComposer(bookingWithGuestsComposer);

                // composing tracked records
                var trackedRecordsBlocks = bookingsWithGuests
                    .Select(booking => booking.Rooms
                        .SelectMany(
                            room => room.Guests,
                            (room, guest) => new TrackedRecord()
                            {
                                Arrival = DateTime.ParseExact(room.Arrival, "yyyyMMdd", null),
                                Nights = (ushort)(DateTime.ParseExact(room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(room.Arrival, "yyyyMMdd", null)).Days,
                                Surname = guest.LastName,
                                Name = guest.FirstName,
                                Sex = guest.Gender switch
                                {
                                    Guest.Sex.M => TrackedRecord.Gender.Male,
                                    Guest.Sex.F => TrackedRecord.Gender.Female,
                                    _ => throw new Exception("Gender was not found")
                                },
                                BirthDate = DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null),
                                PlaceOfBirth = guest.BirthCountry != null && guest.BirthCountry == "IT" && guest.BirthCity != null ? placeConverter.GetPlaceCodeByDescription(guest.BirthCity) : null,
                                ProvinceOfBirth = guest.BirthCounty,
                                StateOfBirth = nationConverter.GetCodeByIso(guest.BirthCountry ?? "IT"),
                                Citizenship = nationConverter.GetCodeByIso(guest.Citizenship ?? "IT"),
                                DocType = guest.DocType switch
                                {
                                    Guest.DocumentType.ID => TrackedRecord.DocumentType.Ident,
                                    Guest.DocumentType.PP => TrackedRecord.DocumentType.Pasor,
                                    Guest.DocumentType.DL => TrackedRecord.DocumentType.Paten,
                                    _ => null
                                },
                                DocNumber = guest.DocNumber,
                                DocIssuer = guest.DocCity != null
                                    ? placeConverter.GetPlaceCodeByDescription(guest.DocCity) ?? (guest.DocCountry != null ? nationConverter.GetCodeByIso(guest.DocCountry) : null)
                                    : guest.DocCountry != null ? nationConverter.GetCodeByIso(guest.DocCountry) : null
                            }));

                var recordBlocksWithCorrectPlaceOfBirth = trackedRecordsBlocks
                    .Select(trackedRecords => trackedRecords
                        .Where(record => record.StateOfBirth != 100000100 || record.PlaceOfBirth != null && record.ProvinceOfBirth?.Length == 2)
                        .ToList())
                    .ToList();

                foreach (var block in recordBlocksWithCorrectPlaceOfBirth)
                {
                    foreach (var record in block)
                    {
                        _accomodatedTypeSolver.Solve(record, block);
                    }
                }

                var correctRecords = recordBlocksWithCorrectPlaceOfBirth
                    .Where(block => block
                        .Where(record =>
                            record.Type == TrackedRecord.AccomodatedType.SingleGuest && block.Count == 1 ||
                            record.Type == TrackedRecord.AccomodatedType.FamilyHead ||
                            record.Type == TrackedRecord.AccomodatedType.GroupHead
                         )
                        .Any()
                    )
                    .SelectMany(
                        block => block,
                        (block, record) => record
                     );

                var result = correctRecords
                    .Select(record => _trackedRecordSerializer.Serialize(record))
                    .ToList();

                await _session.SendDataAsync(result, true);

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        public class SendRequest
        {
            public string Date { get; set; } = "";
        }
    }
}
