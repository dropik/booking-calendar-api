using BookingCalendarApi.Models.Iperbooking.Bookings;
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
        private readonly IAssignedBookingWithGuestsProvider _bookingWithGuestsProvider;
        private readonly DataContext _dataContext;
        private readonly ITrackedRecordsComposer _trackedRecordsComposer;
        private readonly BookingCalendarContext _context;

        private List<AssignedBooking<Guest>> AssignedBookingsWithGuests { get; set; } = new();

        public PoliceController(
            IAlloggiatiServiceSession session,
            IAssignedBookingWithGuestsProvider bookingWithGuestsProvider,
            DataContext dataContext,
            ITrackedRecordsComposer trackedRecordsComposer,
            BookingCalendarContext context)
        {
            _session = session;
            _bookingWithGuestsProvider = bookingWithGuestsProvider;
            _dataContext = dataContext;
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
            
            return AssignedBookingsWithGuests
                .SelectByArrival(DateTime.ParseExact(date, "yyyy-MM-dd", null))
                .UseComposer(_trackedRecordsComposer)
                .ToList();
        }

        private async Task ContextBoundStuff(string date)
        {
            AssignedBookingsWithGuests = await _bookingWithGuestsProvider.Get(date);
            _dataContext.Nations.AddRange(await _context.Nations.ToListAsync());
        }

        private async Task FetchPlaces()
        {
            _dataContext.Places.AddRange(await _session.GetPlacesAsync());
        }

        public class SendRequest
        {
            public string Date { get; set; } = "";
        }
    }
}
