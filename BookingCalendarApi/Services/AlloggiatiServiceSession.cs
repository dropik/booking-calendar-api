using AlloggiatiService;
using BookingCalendarApi.Models.AlloggiatiService;
using Sylvan.Data;
using Sylvan.Data.Csv;

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

        public async Task OpenAsync()
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
                throw new Exception(response.Body.result.ErroreDettaglio);
            }

            Token = response.Body.GenerateTokenResult.token;
        }

        public async Task SendDataAsync(IList<string> data, bool test)
        {
            var array = new ArrayOfString();
            array.AddRange(data);

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
                    throw new Exception(response.Body.TestResult.ErroreDettaglio);
                }
                foreach (var result in response.Body.result.Dettaglio)
                {
                    if (!result.esito)
                    {
                        throw new Exception(result.ErroreDettaglio);
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
                    throw new Exception(response.Body.SendResult.ErroreDettaglio);
                }
                foreach (var result in response.Body.result.Dettaglio)
                {
                    if (!result.esito)
                    {
                        throw new Exception(result.ErroreDettaglio);
                    }
                }
            }
        }

        public async Task<byte[]> GetRicevutaAsync(DateTime date)
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
                throw new Exception(response.Body.RicevutaResult.ErroreDettaglio);
            }
            return response.Body.PDF;
        }

        public async Task<List<Place>> GetPlacesAsync(TipoTabella tipoTabella)
        {
            var tableDataStr = await GetTableAsync(tipoTabella);
            using var textReader = new StringReader(tableDataStr);
            using var csvReader = CsvDataReader.Create(textReader);
            return csvReader.GetRecords<Place>().ToList();
        }

        private async Task<string> GetTableAsync(TipoTabella tipoTabella)
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
                throw new Exception(response.Body.TabellaResult.ErroreDettaglio);
            }
            return response.Body.CSV;
        }
    }
}
