using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PoliceController : ControllerBase
    {
        private readonly IAlloggiatiServiceSession _session;
        private readonly IAlloggiatiTableReader _tableReader;
        private readonly Func<IEnumerable<Place>, IPlaceConverter> _placeConverterProvider;
        private readonly INationConverterProvider _nationConverterProvider;
        private readonly Func<INationConverter, IPlaceConverter, ITrackedRecordsComposer> _trackedRecordsComposerProvider;
        private readonly ITrackedRecordSerializer _trackedRecordSerializer;
        private readonly IBookingWithGuestsProvider _bookingWithGuestsProvider;

        public PoliceController(
            IAlloggiatiServiceSession session,
            IAlloggiatiTableReader tableReader,
            Func<IEnumerable<Place>, IPlaceConverter> placeConverterProvider,
            INationConverterProvider nationConverterProvider,
            Func<INationConverter, IPlaceConverter, ITrackedRecordsComposer> trackedRecordsComposerProvider,
            ITrackedRecordSerializer trackedRecordSerializer,
            IBookingWithGuestsProvider bookingWithGuestsProvider
        )
        {
            _session = session;
            _tableReader = tableReader;
            _placeConverterProvider = placeConverterProvider;
            _nationConverterProvider = nationConverterProvider;
            _trackedRecordsComposerProvider = trackedRecordsComposerProvider;
            _trackedRecordSerializer = trackedRecordSerializer;
            _bookingWithGuestsProvider = bookingWithGuestsProvider;
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
            await _bookingWithGuestsProvider.FetchAsync(date);
            await _nationConverterProvider.FetchAsync();
            
            await _session.OpenAsync();
            var placesStr = await _session.GetTableAsync(AlloggiatiService.TipoTabella.Luoghi);
            var places = _tableReader.ReadAsPlaces(placesStr);
            var placeConverter = _placeConverterProvider(places);

            var recordsComposer = _trackedRecordsComposerProvider(_nationConverterProvider.Converter, placeConverter);

            return _bookingWithGuestsProvider.Bookings
                .UseComposer(recordsComposer)
                .Select(record => _trackedRecordSerializer.Serialize(record))
                .ToList();
        }

        public class SendRequest
        {
            public string Date { get; set; } = "";
        }
    }
}
