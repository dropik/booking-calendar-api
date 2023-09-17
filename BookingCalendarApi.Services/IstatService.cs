using BookingCalendarApi.Models.Clients.C59;
using BookingCalendarApi.Models.DTO;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class IstatService : IIstatService
    {
        private readonly IStructureService _structureService;
        private readonly IAssignedBookingWithGuestsProvider _bookingsProvider;
        private readonly IRepository _repository;
        private readonly IC59Client _c59Client;

        public IstatService(IStructureService structureService, IAssignedBookingWithGuestsProvider bookingsProvider, IRepository context, IC59Client c59Client)
        {
            _structureService = structureService;
            _bookingsProvider = bookingsProvider;
            _repository = context;
            _c59Client = c59Client;
        }

        public async Task<IstatMovementsDTO> GetMovements()
        {
            try
            {
                var credentials = await _structureService.GetC59Credentials();
                var lastUploadResponse = await _c59Client.UltimoC59Async(new UltimoC59Request()
                {
                    Username = credentials.Username,
                    Password = credentials.Password,
                    Struttura = credentials.Struttura,
                });
                var lastUpload = lastUploadResponse.@Return.ElencoC59
                    .OrderBy(item => item.DataMovimentazione.ToString("yyyy-MM-dd"))
                    .First();

                var nations = await _repository.Nations.ToListAsync();

                var date = lastUpload.DataMovimentazione.AddDays(1);

                var bookings = await _bookingsProvider.Get(date.ToString("yyyy-MM-dd"), date.AddDays(1).ToString("yyyy-MM-dd"), exactPeriod: false);

                var prevTotal = lastUpload.TotalePresenti;
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
                    .Select(group => new MovimentoWSPO()
                    {
                        Italia = group.First().IsItaly,
                        Targa = group.Key,
                        Arrivi = group.Where(item => item.Arrival == dateStr).Count(),
                        Partenze = group.Where(item => item.Departure == dateStr).Count()
                    })
                    .ToList();

                return new IstatMovementsDTO()
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
                var totalArrived = movements.Movements.Sum(movement => movement.Arrivi);
                var totalDepartured = movements.Movements.Sum(movement => movement.Partenze);
                var total = movements.PrevTotal + totalArrived - totalDepartured;

                if (movements.Movements.GroupBy(m => m.Targa).Any(g => g.Count() > 1))
                {
                    throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Duplicated entries found.");
                }

                foreach (var movement in movements.Movements)
                {
                    if (movement.Arrivi < 0)
                    {
                        throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Arrivals can not be negative.");
                    }
                    if (movement.Partenze < 0)
                    {
                        throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Departures can not be negative.");
                    }
                    if (movement.Arrivi == 0 && movement.Partenze == 0)
                    {
                        throw new BookingCalendarException(BCError.INVALID_ISTAT_MOVEMENTS, "Either arrivals or departures must be set.");
                    }
                }

                movements.Movements = movements.Movements
                    .Select(m => new MovimentoWSPO()
                    {
                        Italia = m.Italia,
                        Targa = m.Targa.ToUpperInvariant(),
                        Arrivi = m.Arrivi,
                        Partenze = m.Partenze,
                    })
                    .ToList();

                var credentials = await _structureService.GetC59Credentials();
                var c59Request = new InviaC59FullRequest()
                {
                    Username = credentials.Username,
                    Password = credentials.Password,
                    Struttura = credentials.Struttura,
                    C59 = new C59WSPO()
                    {
                        DataMovimentazione = DateTime.ParseExact(movements.Date, "yyyy-MM-dd", null),
                        DataMovimentazioneSpecified = true,
                        EsercizioAperto = true,
                        TotaleArrivi = totalArrived,
                        TotalePartenze = totalDepartured,
                        TotalePresenti = total,
                        UnitaAbitativeDisponibili = 11,     // that's hardcoded, does not seem to be used precedently
                        UnitaAbitativeOccupate = 0,         // that's also hardcoded
                        Movimenti = movements.Movements.ToArray(),

                    },
                };

                await _c59Client.InviaC59FullAsync(c59Request);
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
            var countries = await (from nation in _repository.Nations
                                   select nation.Description).ToListAsync();
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
                    result += $"{item[0]}{item.Substring(1).ToLowerInvariant()} ";
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
                    result += $"'{$"{item[0]}".ToUpperInvariant()}{item.Substring(1)}";
                }
            }

            split = result.Split('.');
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
                    result += $".{$"{item[0]}".ToUpperInvariant()}{item.Substring(1)}";
                }
            }

            return result;
        }
    }
}
