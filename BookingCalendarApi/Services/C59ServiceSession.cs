using BookingCalendarApi.Models.C59Service;
using C59Service;

namespace BookingCalendarApi.Services
{
    public class C59ServiceSession : IC59ServiceSession
    {
        private readonly EC59ServiceEndpoint _service;
        private readonly Credentials _credentials;

        public C59ServiceSession(EC59ServiceEndpoint service, IConfiguration configuration)
        {
            _service = service;
            _credentials = configuration.GetSection("C59Service").Get<Credentials>();
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

        public async Task SendNewDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
