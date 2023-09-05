using BookingCalendarApi.Exceptions;
using BookingCalendarApi.Models.C59Service;
using BookingCalendarApi.Models.DTO;
using C59Service;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel;

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

        public async Task<IstatMovementsDTO> GetMovements()
        {
            try
            {
                var lastUploadRequest = new ultimoC59(_credentials.Username, _credentials.Password, _credentials.Struttura);
                var lastUploadResponse = await _service.ultimoC59Async(lastUploadRequest);
                var lastUpload = lastUploadResponse.@return.elencoC59
                    .OrderBy(item => item.dataMovimentazione.ToString("yyyy-MM-dd"))
                    .First();

                var nations = await _context.Nations.ToListAsync();

                var date = lastUpload.dataMovimentazione.AddDays(1);

                var bookings = await _bookingsProvider.Get(date.ToString("yyyy-MM-dd"), date.AddDays(1).ToString("yyyy-MM-dd"), exactPeriod: false);

                var prevTotal = lastUpload.totalePresenti;
                var dateStr = date.ToString("yyyyMMdd");
                var arrivedOrDeparturedStays = bookings
                    .SelectMany(
                        booking => booking.Rooms
                            .Where(room => room.Arrival == dateStr || room.Departure == dateStr),
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
                            IsItaly = guest.BirthCountry == "IT",
                            Targa = guest.BirthCountry == "IT"
                                ? guest.BirthCounty
                                : DecapitalizeCountryName(nations.SingleOrDefault(nation => nation.Iso == guest.BirthCountry)?.Description ?? ""),
                        }
                    );

                var movements = guestsWithProvinceOrState
                    .GroupBy(guest => guest.Targa)
                    .Select(group => new movimentoWSPO()
                    {
                        italia = group.First().IsItaly,
                        targa = group.Key,
                        arrivi = group.Where(item => item.Arrival == dateStr).Count(),
                        partenze = group.Where(item => item.Departure == dateStr).Count()
                    })
                    .ToList();

                return new()
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    PrevTotal = prevTotal,
                    Movements = movements,
                };
            }
            catch (FaultException exception)
            {
                throw new BookingCalendarException(BCError.ISTAT_ERROR, $"Error occured while retreiving data from ISTAT: {exception.Message}");
            }
            catch (CommunicationException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed establish connection with ISTAT service: {exception.Message}");
            }
        }

        public async Task SendMovements(IstatMovementsDTO movements)
        {
            try
            {
                var totalArrived = movements.Movements.Sum(movement => movement.arrivi);
                var totalDepartured = movements.Movements.Sum(movement => movement.partenze);
                var total = movements.PrevTotal + totalArrived - totalDepartured;

                if (movements.Movements.GroupBy(m => m.targa).Any(g => g.Count() > 1))
                {
                    throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Duplicated entries found.");
                }

                foreach (var movement in movements.Movements)
                {
                    if (movement.arrivi < 0)
                    {
                        throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Arrivals can not be negative.");
                    }
                    if (movement.partenze < 0)
                    {
                        throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Departures can not be negative.");
                    }
                    if (movement.arrivi == 0 && movement.partenze == 0)
                    {
                        throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Either arrivals or departures must be set.");
                    }
                }

                movements.Movements = movements.Movements
                    .Select(m => new movimentoWSPO()
                    {
                        italia = m.italia,
                        targa = m.targa.ToUpperInvariant(),
                        arrivi = m.arrivi,
                        partenze = m.partenze,
                    })
                    .ToList();

                var c59Request = new inviaC59Full(_credentials.Username, _credentials.Password, _credentials.Struttura, new c59WSPO()
                {
                    dataMovimentazione = DateTime.ParseExact(movements.Date, "yyyy-MM-dd", null),
                    dataMovimentazioneSpecified = true,
                    esercizioAperto = true,
                    totaleArrivi = totalArrived,
                    totalePartenze = totalDepartured,
                    totalePresenti = total,
                    unitaAbitativeDisponibili = 11,     // that's hardcoded, does not seem to be used precedently
                    unitaAbitativeOccupate = 0,         // that's also hardcoded
                    movimenti = movements.Movements.ToArray()
                });

                await _service.inviaC59FullAsync(c59Request);
            }
            catch (FaultException exception)
            {
                throw new BookingCalendarException(BCError.ISTAT_ERROR, $"Error occured while sending data to ISTAT: {exception.Message}");
            }
            catch (CommunicationException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed establish connection with ISTAT service: {exception.Message}");
            }
        }

        public async Task<List<string>> GetCountries()
        {
            var countries = await (from nation in _context.Nations
                                   select nation.Description
                                  ).ToListAsync();
            return countries.Distinct().Select(c => DecapitalizeCountryName(c)).OrderBy(c => c).ToList();
        }

        private static string DecapitalizeCountryName(string name)
        {
            if (name == null)
            {
                return "";
            }

            if (name.Length <= 3)
            {
                return name;
            }

            var split = name.Split(' ');
            var result = "";
            foreach (var item in split)
            {
                if (item == "E")
                {
                    result += "e ";
                }
                else
                {
                    result += $"{item[..1]}{item[1..].ToLowerInvariant()} ";
                }
            }
            result = result.Remove(result.Length - 1);

            split = result.Split('\'');
            result = split[0];
            for (var i = 1; i < split.Length; i++)
            {
                var item = split[i];
                if (item.Length == 0)
                {
                    result += "'";
                }
                else
                {
                    result += $"'{item[..1].ToUpperInvariant()}{item[1..]}";
                }
            }

            split = result.Split(".");
            result = split[0];
            for (var i = 1; i < split.Length; i++)
            {
                var item = split[i];
                if (item.Length == 0)
                {
                    result += ".";
                }
                else
                {
                    result += $".{item[..1].ToUpperInvariant()}{item[1..]}";
                }
            }

            return result;
        }
    }
}
