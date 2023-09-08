using BookingCalendarApi.Models.Clients.Police;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IPoliceClient
    {
        Task<GenerateTokenResponse> GenerateTokenAsync(GenerateTokenRequest request);
        Task<TestResponse> TestAsync(TestRequest request);
        Task<SendResponse> SendAsync(SendRequest request);
        Task<RicevutaResponse> RicevutaAsync(RicevutaRequest request);
        Task<TabellaResponse> TabellaAsync(TabellaRequest request);
    }
}
