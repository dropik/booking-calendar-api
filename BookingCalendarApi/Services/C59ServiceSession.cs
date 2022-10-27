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

        public async Task SendAsync(c59WSPO data)
        {
            var request = new inviaC59Full(_credentials.Username, _credentials.Password, _credentials.Struttura, data);
            await _service.inviaC59FullAsync(request);
        }
    }
}
