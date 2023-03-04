using AlloggiatiService;
using BookingCalendarApi.Exceptions;
using BookingCalendarApi.Models.AlloggiatiService;
using Sylvan.Data;
using Sylvan.Data.Csv;
using System.ServiceModel;

namespace BookingCalendarApi.Services
{
    public class AlloggiatiServiceSession : IAlloggiatiServiceSession
    {
        private readonly Credentials _credentials;
        private readonly IServiceSoap _service;

        private string Token { get; set; } = "";

        public AlloggiatiServiceSession(IConfiguration configuration, IServiceSoap service)
        {
            _credentials = configuration.GetSection("AlloggiatiService").Get<Credentials>();
            _service = service;
        }

        public async Task Open()
        {
            try
            {

                var request = new GenerateTokenRequest(new GenerateTokenRequestBody()
                {
                    Utente = _credentials.Utente,
                    Password = _credentials.Password,
                    WsKey = _credentials.WsKey
                });
                var response = await _service.GenerateTokenAsync(request);
                if (!response.Body.result.esito)
                {
                    throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.result.ErroreDettaglio);
                }

                Token = response.Body.GenerateTokenResult.token;
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
                    var request = new TestRequest(new TestRequestBody()
                    {
                        Utente = _credentials.Utente,
                        token = Token,
                        ElencoSchedine = array
                    });
                
                    var response = await _service.TestAsync(request);
                    if (!response.Body.TestResult.esito)
                    {
                        throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.TestResult.ErroreDettaglio);
                    }
                    foreach (var result in response.Body.result.Dettaglio)
                    {
                        if (!result.esito)
                        {
                            throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, result.ErroreDettaglio);
                        }
                    }
                
                }
                else
                {
                    var request = new SendRequest(new SendRequestBody()
                    {
                        Utente = _credentials.Utente,
                        token = Token,
                        ElencoSchedine = array
                    });
                    var response = await _service.SendAsync(request);
                    if (!response.Body.SendResult.esito)
                    {
                        throw new BookingCalendarException(BCError.POLICE_SERVICE_ERROR, response.Body.SendResult.ErroreDettaglio);
                    }
                    foreach (var result in response.Body.result.Dettaglio)
                    {
                        if (!result.esito)
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

                var request = new RicevutaRequest(new RicevutaRequestBody()
                {
                    Utente = _credentials.Utente,
                    token = Token,
                    Data = date
                });
                var response = await _service.RicevutaAsync(request);
                if (!response.Body.RicevutaResult.esito)
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
            var tableDataStr = await GetTable(TipoTabella.Luoghi);
            using var textReader = new StringReader(tableDataStr);
            using var csvReader = CsvDataReader.Create(textReader);
            return csvReader.GetRecords<Place>().ToList();
        }

        private async Task<string> GetTable(TipoTabella tipoTabella)
        {
            try
            {
                var request = new TabellaRequest(new TabellaRequestBody()
                {
                    Utente = _credentials.Utente,
                    token = Token,
                    tipo = tipoTabella
                });
                var response = await _service.TabellaAsync(request);
                if (!response.Body.TabellaResult.esito)
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
