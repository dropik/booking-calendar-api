using BookingCalendarApi.Models.Clients.C59;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IC59Client
    {
        Task<UltimoC59Response> UltimoC59Async(UltimoC59Request request);
        Task InviaC59FullAsync(InviaC59FullRequest request);
    }
}
