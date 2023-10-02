using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class PoliceService : IPoliceService
    {
        private readonly IAlloggiatiServiceSession _session;
        private readonly IAssignedBookingWithGuestsProvider _bookingWithGuestsProvider;
        private readonly DataContext _dataContext;
        private readonly ITrackedRecordsComposer _trackedRecordsComposer;
        private readonly IRepository _repository;
        private readonly ApiSettings _apiSettings;

        public PoliceService(
            IAlloggiatiServiceSession session,
            IAssignedBookingWithGuestsProvider bookingWithGuestsProvider,
            DataContext dataContext,
            ITrackedRecordsComposer trackedRecordsComposer,
            IRepository repository,
            IOptions<ApiSettings> apiSettings)
        {
            _session = session;
            _bookingWithGuestsProvider = bookingWithGuestsProvider;
            _dataContext = dataContext;
            _trackedRecordsComposer = trackedRecordsComposer;
            _repository = repository;
            _apiSettings = apiSettings.Value;
        }

        public async Task<PoliceRicevutaResponse> GetRicevuta(string date)
        {
            await _session.Open();
            var pdf = await _session.GetRicevuta(DateTime.ParseExact(date, "yyyy-MM-dd", null));
            return new PoliceRicevutaResponse()
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

            if (_apiSettings.Environment == Models.Configurations.Environment.Production)
            {
                await _session.SendData(records, false);       // if no exception occured - send
            }
        }

        public async Task<List<string>> GetProvinces()
        {
            await _session.Open();
            var places = await _session.GetPlaces();
            return places
                .Select(p => p.Provincia)
                .Distinct()
                .Except(new List<string>() { "ES" })
                .OrderBy(p => p)
                .ToList();
        }

        private async Task<List<string>> ComposeRecords(string date)
        {
            _dataContext.Nations.AddRange(await _repository.Nations.ToListAsync());
            _dataContext.Places.AddRange(await _session.GetPlaces());
            var assignedBookingsWithGuests = await _bookingWithGuestsProvider.Get(date);

            return assignedBookingsWithGuests
                .SelectByArrival(DateTime.ParseExact(date, "yyyy-MM-dd", null))
                .UseComposer(_trackedRecordsComposer)
                .ToList();
        }
    }
}
