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
        private readonly Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer> _assignedBookingComposerProvider;
        private readonly Func<IEnumerable<Reservation>, IBookingWithGuestsComposer> _bookingWithGuestsComposerProvider;
        private readonly BookingCalendarContext _context;
        private readonly IAlloggiatiTableReader _tableReader;
        private readonly Func<IEnumerable<Place>, IPlaceConverter> _placeConverterProvider;
        private readonly Func<IEnumerable<PoliceNationCode>, INationConverter> _nationConverterProvider;
        private readonly Func<INationConverter, IPlaceConverter, ITrackedRecordsComposer> _trackedRecordsComposerProvider;
        private readonly ITrackedRecordSerializer _trackedRecordSerializer;

        public PoliceController(
            IAlloggiatiServiceSession session,
            IBookingsProvider bookingsProvider,
            IIperbooking iperbooking,
            Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer> assignedBookingsComposerProvider,
            Func<IEnumerable<Reservation>, IBookingWithGuestsComposer> bookingWithGuestsComposerProvider,
            BookingCalendarContext context,
            IAlloggiatiTableReader tableReader,
            Func<IEnumerable<Place>, IPlaceConverter> placeConverterProvider,
            Func<IEnumerable<PoliceNationCode>, INationConverter> nationConverterProvider,
            Func<INationConverter, IPlaceConverter, ITrackedRecordsComposer> trackedRecordsComposerProvider,
            ITrackedRecordSerializer trackedRecordSerializer
        )
        {
            _session = session;
            _bookingsProvider = bookingsProvider;
            _iperbooking = iperbooking;
            _assignedBookingComposerProvider = assignedBookingsComposerProvider;
            _bookingWithGuestsComposerProvider = bookingWithGuestsComposerProvider;
            _context = context;
            _tableReader = tableReader;
            _placeConverterProvider = placeConverterProvider;
            _nationConverterProvider = nationConverterProvider;
            _trackedRecordsComposerProvider = trackedRecordsComposerProvider;
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
                var records = await ComposeRecordsAsync(request.Date);
                await _session.SendDataAsync(records, true);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendAsync(SendRequest request)
        {
            try
            {
                var records = await ComposeRecordsAsync(request.Date);
                await _session.SendDataAsync(records, true);        // test it first
                await _session.SendDataAsync(records, false);       // if no exception occured - send
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        private async Task<List<string>> ComposeRecordsAsync(string date)
        {
            var to = DateTime.ParseExact(date, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");

            await _bookingsProvider.FetchBookingsAsync(date, to, exactPeriod: true);
            var bookings = _bookingsProvider.Bookings
                .ExcludeCancelled();

            var stayIds = bookings
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => $"{room.StayId}-{room.Arrival}-{room.Departure}"
                );

            var assignments = await _context.RoomAssignments
                .Where(assignment => stayIds.Contains(assignment.Id))
                .ToListAsync();

            var assignedBookingComposer = _assignedBookingComposerProvider(assignments);

            var assignedBookings = bookings
                .UseComposer(assignedBookingComposer)
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

            var recordsComposer = _trackedRecordsComposerProvider(nationConverter, placeConverter);
            var correctRecords = bookingsWithGuests.UseComposer(recordsComposer);

            return correctRecords
                .Select(record => _trackedRecordSerializer.Serialize(record))
                .ToList();
        }

        public class SendRequest
        {
            public string Date { get; set; } = "";
        }
    }
}
