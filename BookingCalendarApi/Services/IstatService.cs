using BookingCalendarApi.Exceptions;
using BookingCalendarApi.Models.C59Service;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using C59Service;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class IstatService : IIstatService
    {
        private readonly EC59ServiceEndpoint _service;
        private readonly Credentials _credentials;
        private readonly IAssignedBookingWithGuestsProvider _bookingsProvider;
        private readonly BookingCalendarContext _context;

        public IstatService(EC59ServiceEndpoint service, IConfiguration configuration, IAssignedBookingWithGuestsProvider bookingsProvider, BookingCalendarContext context)
        {
            _service = service;
            _credentials = configuration.GetSection("C59Service").Get<Credentials>();
            _bookingsProvider = bookingsProvider;
            _context = context;
        }

        public async Task<IstatLastDateResponse> GetLastDate()
        {
            var request = new ultimoC59(_credentials.Username, _credentials.Password, _credentials.Struttura);
            var response = await _service.ultimoC59Async(request);
            var lastDate = response.@return.elencoC59
                .Select(item => item.dataMovimentazione.ToString("yyyy-MM-dd"))
                .OrderBy(date => date)
                .First();
            return new()
            {
                LastDate = lastDate,
            };
        }

        public async Task SendNewData(IstatSendDataRequest request)
        {
            var to = DateTime.ParseExact(request.Date, "yyyy-MM-dd", null);
            var lastUploadRequest = new ultimoC59(_credentials.Username, _credentials.Password, _credentials.Struttura);
            var lastUploadResponse = await _service.ultimoC59Async(lastUploadRequest);
            var lastUpload = lastUploadResponse.@return.elencoC59
                .OrderBy(item => item.dataMovimentazione.ToString("yyyy-MM-dd"))
                .First();

            var nations = await _context.Nations.ToListAsync();

            var fromDate = lastUpload.dataMovimentazione.AddDays(1);
            if ((to - fromDate).Days < 0)
            {
                throw new BookingCalendarException(BCError.ISTAT_ERROR, "Overriding ISTAT history is not possible.");
            }

            var bookings = await _bookingsProvider.Get(fromDate.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"), exactPeriod: false);

            var prevTotal = lastUpload.totalePartenze;
            var dateCounter = fromDate;
            while ((to - dateCounter).Days >= 0)
            {
                var date = dateCounter.ToString("yyyyMMdd");
                var arrivedOrDeparturedStays = bookings
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
                            targa = nations.SingleOrDefault(nation => nation.Iso == group.Key)?.Description ?? "",
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

                var totalArrived = movements.Sum(movement => movement.arrivi);
                var totalDepartured = movements.Sum(movement => movement.partenze);
                var total = prevTotal + totalArrived - totalDepartured;

                var c59Request = new inviaC59Full(_credentials.Username, _credentials.Password, _credentials.Struttura, new c59WSPO()
                {
                    dataMovimentazione = dateCounter,
                    dataMovimentazioneSpecified = true,
                    esercizioAperto = true,
                    totaleArrivi = totalArrived,
                    totalePartenze = totalDepartured,
                    totalePresenti = total,
                    unitaAbitativeDisponibili = 11,
                    unitaAbitativeOccupate = 0,
                    movimenti = movements.ToArray()
                });

                await _service.inviaC59FullAsync(c59Request);

                prevTotal = total;
                dateCounter = dateCounter.AddDays(1);
            }
        }
    }
}
