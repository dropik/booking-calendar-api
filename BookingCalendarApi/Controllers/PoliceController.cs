using BookingCalendarApi.Models;
using BookingCalendarApi.Models.AlloggiatiService;
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
        private readonly IAssignedBookingWithGuestsProvider _bookingWithGuestsProvider;
        private readonly List<Place> _places;
        private readonly List<Nation> _nations;
        private readonly ITrackedRecordsComposer _trackedRecordsComposer;
        private readonly BookingCalendarContext _context;

        public PoliceController(
            IAlloggiatiServiceSession session,
            IAssignedBookingWithGuestsProvider bookingWithGuestsProvider,
            List<Place> places,
            List<Nation> nations,
            ITrackedRecordsComposer trackedRecordsComposer,
            BookingCalendarContext context)
        {
            _session = session;
            _bookingWithGuestsProvider = bookingWithGuestsProvider;
            _places = places;
            _nations = nations;
            _trackedRecordsComposer = trackedRecordsComposer;
            _context = context;
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
                await _session.OpenAsync();
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
                await _session.OpenAsync();
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
                ContextBoundStuff(date),
                FetchPlaces()
            );
            
            return _bookingWithGuestsProvider.Bookings
                .SelectByArrival(DateTime.ParseExact(date, "yyyy-MM-dd", null))
                .UseComposer(_trackedRecordsComposer)
                .ToList();
        }

        private async Task ContextBoundStuff(string date)
        {
            await _bookingWithGuestsProvider.FetchAsync(date);
            _nations.Clear();
            _nations.AddRange(await _context.Nations.ToListAsync());
        }

        private async Task FetchPlaces()
        {
            _places.Clear();
            _places.AddRange(await _session.GetPlacesAsync());
        }

        public class SendRequest
        {
            public string Date { get; set; } = "";
        }
    }
}
