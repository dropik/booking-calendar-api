using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PoliceController : ControllerBase
    {
        private readonly IAlloggiatiServiceSession _session;
        private readonly IAssignedBookingWithGuestsProvider _bookingWithGuestsProvider;
        private readonly INationConverterProvider _nationConverterProvider;
        private readonly IPlaceConverterProvider _placeConverterProvider;
        private readonly Func<INationConverter, IPlaceConverter, ITrackedRecordsComposer> _trackedRecordsComposerProvider;

        public PoliceController(
            IAlloggiatiServiceSession session,
            IAssignedBookingWithGuestsProvider bookingWithGuestsProvider,
            INationConverterProvider nationConverterProvider,
            IPlaceConverterProvider placeConverterProvider,
            Func<INationConverter, IPlaceConverter, ITrackedRecordsComposer> trackedRecordsComposerProvider
        )
        {
            _session = session;
            _bookingWithGuestsProvider = bookingWithGuestsProvider;
            _nationConverterProvider = nationConverterProvider;
            _placeConverterProvider = placeConverterProvider;
            _trackedRecordsComposerProvider = trackedRecordsComposerProvider;
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
            await Task.WhenAll(
                _bookingWithGuestsProvider.FetchAsync(date),
                _nationConverterProvider.FetchAsync(),
                _placeConverterProvider.FetchAsync(_session)
            );
            
            var recordsComposer = _trackedRecordsComposerProvider(_nationConverterProvider.Converter, _placeConverterProvider.Converter);

            return _bookingWithGuestsProvider.Bookings
                .SelectByArrival(DateTime.ParseExact(date, "yyyy-MM-dd", null))
                .UseComposer(recordsComposer)
                .ToList();
        }

        public class SendRequest
        {
            public string Date { get; set; } = "";
        }
    }
}
