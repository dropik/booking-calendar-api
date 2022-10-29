﻿using BookingCalendarApi.Models;
using BookingCalendarApi.Models.C59Service;
using C59Service;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class C59ServiceSession : IC59ServiceSession
    {
        private readonly EC59ServiceEndpoint _service;
        private readonly Credentials _credentials;
        private readonly IAssignedBookingWithGuestsProvider _bookingsProvider;
        private readonly BookingCalendarContext _context;

        public C59ServiceSession(EC59ServiceEndpoint service, IConfiguration configuration, IAssignedBookingWithGuestsProvider bookingsProvider, BookingCalendarContext context)
        {
            _service = service;
            _credentials = configuration.GetSection("C59Service").Get<Credentials>();
            _bookingsProvider = bookingsProvider;
            _context = context;
        }

        public async Task<string> GetLastDateAsync()
        {
            var request = new ultimoC59(_credentials.Username, _credentials.Password, _credentials.Struttura);
            var response = await _service.ultimoC59Async(request);
            return response.@return.elencoC59
                .Select(item => item.dataMovimentazione.ToString("yyyy-MM-dd"))
                .OrderBy(date => date)
                .First();
        }

        public async Task<IEnumerable<MovementsTestResponseItem>> SendNewDataAsync()
        {
            var lastUploadRequest = new ultimoC59(_credentials.Username, _credentials.Password, _credentials.Struttura);
            var lastUploadResponse = await _service.ultimoC59Async(lastUploadRequest);
            var lastUpload = lastUploadResponse.@return.elencoC59
                .OrderBy(item => item.dataMovimentazione.ToString("yyyy-MM-dd"))
                .First();

            var nations = await _context.Nations.ToListAsync();

            var fromDate = lastUpload.dataMovimentazione.AddDays(1);
            var toDate = DateTime.ParseExact(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), "yyyyMMdd", null); // always publish up to yesterday
            await _bookingsProvider.FetchAsync(fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), exactPeriod: false);

            var prevTotal = lastUpload.totalePartenze;
            var dateCounter = fromDate;
            var result = new List<MovementsTestResponseItem>();
            while ((toDate - dateCounter).Days >= 0)
            {
                var date = dateCounter.ToString("yyyyMMdd");
                var arrivedOrDeparturedStays = _bookingsProvider.Bookings
                    .SelectMany(
                        booking => booking.Rooms
                            .Where(room => room.Arrival == date || room.Departure == date),
                        (booking, room) => room
                    );
                var guestsWithProvinceOrState = arrivedOrDeparturedStays
                    .SelectMany(
                        stay => stay.Guests
                            .Where(guest =>
                                guest.BirthCountry != null &&
                                guest.BirthCountry.Trim() != "" &&
                                ((guest.BirthCountry != "IT") || (guest.BirthCounty != null && guest.BirthCounty.Trim() != ""))),
                        (stay, guest) => new
                        {
                            stay.Arrival,
                            stay.Departure,
                            Country = guest.BirthCountry,
                            Province = guest.BirthCounty
                        }
                    );
                var movements = guestsWithProvinceOrState
                    .GroupBy(guest => guest.Country)
                    .Select(group => group.Key != "IT" ? new List<movimentoWSPO>() {
                        new movimentoWSPO()
                        {
                            italia = false,
                            targa = nations.SingleOrDefault(nation => nation.Iso == group.Key)?.Description,
                            arrivi = group.Where(item => item.Arrival == date).Count(),
                            partenze = group.Where(item => item.Departure == date).Count()
                        }
                    } : group
                        .GroupBy(item => item.Province)
                        .Select(provinceGroup => new movimentoWSPO()
                        {
                            italia = true,
                            targa = provinceGroup.Key,
                            arrivi = provinceGroup.Where(item => item.Arrival == date).Count(),
                            partenze = provinceGroup.Where(item => item.Departure == date).Count()
                        }))
                    .SelectMany(movements => movements);

                result.Add(new MovementsTestResponseItem(date, movements));
                dateCounter = dateCounter.AddDays(1);
            }

            return result;
        }
    }
}
