using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IStructureService
    {
        Task<Models.Iperbooking.Auth> GetIperbookingCredentials();
        Task<Models.AlloggiatiService.Credentials> GetAlloggiatiServiceCredentials();
        Task<Models.C59Service.Credentials> GetC59Credentials();
        Task<APIKeysResponse> GetApiKeys();
        Task UpdateApiKeys(UpdateAPIKeysRequest request);
    }
}
