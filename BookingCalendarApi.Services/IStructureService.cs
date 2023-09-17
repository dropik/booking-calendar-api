using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IStructureService
    {
        Task<Models.Iperbooking.Auth> GetIperbookingCredentials();
        Task<Models.AlloggiatiService.Credentials> GetAlloggiatiServiceCredentials();
        Task<Models.C59Service.Credentials> GetC59Credentials();
    }
}
