﻿using AlloggiatiService;
using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Exceptions;
using Sylvan.Data;
using Sylvan.Data.Csv;
using System.ServiceModel;

namespace BookingCalendarApi.Services
{
    public class AlloggiatiServiceSession : IAlloggiatiServiceSession
    {
        private readonly Credentials _credentials;
        private readonly IPoliceClient _policeClient;

        private string Token { get; set; } = "";

        public AlloggiatiServiceSession(IConfiguration configuration, IPoliceClient policeClient)
        {
            _credentials = configuration.GetSection("AlloggiatiService").Get<Credentials>();
            _policeClient = policeClient;
        }

        public async Task Open()
        {
            try
            {
                var response = await _policeClient.GenerateTokenAsync(new()
                {
                    Body = new()
                    {
                        Utente = _credentials.Utente,
                        Password = _credentials.Password,
                        WsKey = _credentials.WsKey,
                    }
                });
                if (!response.Body.Result.Esito)
                {
                    throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.Result.ErroreDettaglio);
                }

                Token = response.Body.GenerateTokenResult.Token;
            }
            catch (CommunicationException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed establish connection to police service: {exception.Message}");
            }
        }

        public async Task SendData(IList<string> data, bool test)
        {
            var array = new ArrayOfString();
            array.AddRange(data);

            try
            {
                if (test)
                {
                    var response = await _policeClient.TestAsync(new()
                    {
                        Body = new()
                        {
                            Utente = _credentials.Utente,
                            Token = Token,
                            ElencoSchedine = array,
                        }
                    });
                    if (!response.Body.TestResult.Esito)
                    {
                        throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.TestResult.ErroreDettaglio);
                    }
                    foreach (var result in response.Body.Result.Dettaglio)
                    {
                        if (!result.Esito)
                        {
                            throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, result.ErroreDettaglio);
                        }
                    }
                }
                else
                {
                    var response = await _policeClient.SendAsync(new()
                    {
                        Body = new()
                        {
                            Utente = _credentials.Utente,
                            Token = Token,
                            ElencoSchedine = array,
                        }
                    });
                    if (!response.Body.SendResult.Esito)
                    {
                        throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.SendResult.ErroreDettaglio);
                    }
                    foreach (var result in response.Body.Result.Dettaglio)
                    {
                        if (!result.Esito)
                        {
                            throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, result.ErroreDettaglio);
                        }
                    }
                }
            }
            catch (CommunicationException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed establish connection to police service: {exception.Message}");
            }
        }

        public async Task<byte[]> GetRicevuta(DateTime date)
        {
            try
            {
                var response = await _policeClient.RicevutaAsync(new()
                {
                    Body = new()
                    {
                        Utente = _credentials.Utente,
                        Token = Token,
                        Data = date,
                    }
                });
                if (!response.Body.RicevutaResult.Esito)
                {
                    throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.RicevutaResult.ErroreDettaglio);
                }
                return response.Body.PDF;
            }
            catch (CommunicationException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed establish connection to police service: {exception.Message}");
            }
        }

        public async Task<List<Place>> GetPlaces()
        {
            var tableDataStr = await GetTable(Models.Clients.Police.TipoTabella.Luoghi);
            using var textReader = new StringReader(tableDataStr);
            using var csvReader = CsvDataReader.Create(textReader);
            return csvReader.GetRecords<Place>().ToList();
        }

        private async Task<string> GetTable(Models.Clients.Police.TipoTabella tipoTabella)
        {
            try
            {
                var response = await _policeClient.TabellaAsync(new()
                {
                    Body = new()
                    {
                        Utente = _credentials.Utente,
                        Token = Token,
                        Tipo = tipoTabella
                    }
                });
                if (!response.Body.TabellaResult.Esito)
                {
                    throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.TabellaResult.ErroreDettaglio);
                }
                return response.Body.CSV;
            }
            catch (CommunicationException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed establish connection to police service: {exception.Message}");
            }
        }
    }
}
