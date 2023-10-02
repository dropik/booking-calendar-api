﻿using BookingCalendarApi.Models.Clients.C59;
using BookingCalendarApi.NETFramework.C59Service;
using BookingCalendarApi.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingCalendarApi.NETFramework.Clients
{
    public class C59Client : IC59Client
    {
        private readonly EC59ServiceEndpoint _service;

        public C59Client(EC59ServiceEndpoint service)
        {
            _service = service;
        }

        private static readonly JsonSerializerOptions _deepCopyOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
        };

        public async Task InviaC59FullAsync(InviaC59FullRequest request)
        {
            await _service.inviaC59FullAsync(JsonSerializer.Deserialize<inviaC59Full>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
        }

        public async Task<UltimoC59Response> UltimoC59Async(UltimoC59Request request)
        {
            var response = await _service.ultimoC59Async(JsonSerializer.Deserialize<ultimoC59>(JsonSerializer.Serialize(request, _deepCopyOptions), _deepCopyOptions));
            return JsonSerializer.Deserialize<UltimoC59Response>(JsonSerializer.Serialize(response, _deepCopyOptions), _deepCopyOptions) ?? new UltimoC59Response();
        }
    }
}
