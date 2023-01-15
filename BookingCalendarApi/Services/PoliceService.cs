using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class PoliceService : IPoliceService
    {
        private readonly IAlloggiatiServiceSession _session;
        private readonly IAssignedBookingWithGuestsProvider _bookingWithGuestsProvider;
        private readonly DataContext _dataContext;
        private readonly ITrackedRecordsComposer _trackedRecordsComposer;
        private readonly BookingCalendarContext _context;

        private List<AssignedBooking<Guest>> AssignedBookingsWithGuests { get; set; } = new();

        public PoliceService(
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

        public async Task<PoliceRicevutaResponse> GetRicevuta(string date)
        {
            await _session.Open();
            var pdf = await _session.GetRicevutaAsync(DateTime.ParseExact(date, "yyyy-MM-dd", null));
            return new()
            {
                Pdf = pdf,
                FileName = $"polizia-ricevuta-{date}.pdf",
            };
        }

        public async Task Test(PoliceSendRequest request)
        {
            await _session.Open();
            var records = await ComposeRecords(request.Date);
            await _session.SendData(records, true);
        }

        public async Task Send(PoliceSendRequest request)
        {
            await _session.Open();
            var records = await ComposeRecords(request.Date);
            await _session.SendData(records, true);        // test it first
            await _session.SendData(records, false);       // if no exception occured - send
        }

        private async Task<List<string>> ComposeRecords(string date)
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
    }
}
