using BookingCalendarApi.Models.Clients.Police;
using BookingCalendarApi.Services;
using System.Text.Json;

namespace BookingCalendarApi.Clients
{
    public class PoliceClient : IPoliceClient
    {
        private readonly AlloggiatiService.IServiceSoap _service;

        private static readonly JsonSerializerOptions _deepCopyOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
        };

        public PoliceClient(AlloggiatiService.IServiceSoap service)
        {
            _service = service;
        }

        public async Task<GenerateTokenResponse> GenerateTokenAsync(GenerateTokenRequest request)
        {
            var response = await _service.GenerateTokenAsync(JsonSerializer.Deserialize<AlloggiatiService.GenerateTokenRequest>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
            return JsonSerializer.Deserialize<GenerateTokenResponse>(JsonSerializer.Serialize(response, _deepCopyOptions), _deepCopyOptions) ?? new();
        }

        public async Task<RicevutaResponse> RicevutaAsync(RicevutaRequest request)
        {
            var response = await _service.RicevutaAsync(JsonSerializer.Deserialize<AlloggiatiService.RicevutaRequest>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
            return JsonSerializer.Deserialize<RicevutaResponse>(JsonSerializer.Serialize(response, _deepCopyOptions), _deepCopyOptions) ?? new();
        }

        public async Task<SendResponse> SendAsync(SendRequest request)
        {
            var response = await _service.SendAsync(JsonSerializer.Deserialize<AlloggiatiService.SendRequest>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
            return JsonSerializer.Deserialize<SendResponse>(JsonSerializer.Serialize(response, _deepCopyOptions), _deepCopyOptions) ?? new();
        }

        public async Task<TabellaResponse> TabellaAsync(TabellaRequest request)
        {
            var response = await _service.TabellaAsync(JsonSerializer.Deserialize<AlloggiatiService.TabellaRequest>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
            return JsonSerializer.Deserialize<TabellaResponse>(JsonSerializer.Serialize(response, _deepCopyOptions), _deepCopyOptions) ?? new();
        }

        public async Task<TestResponse> TestAsync(TestRequest request)
        {
            var response = await _service.TestAsync(JsonSerializer.Deserialize<AlloggiatiService.TestRequest>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
            return JsonSerializer.Deserialize<TestResponse>(JsonSerializer.Serialize(response, _deepCopyOptions), _deepCopyOptions) ?? new();
        }
    }
}
