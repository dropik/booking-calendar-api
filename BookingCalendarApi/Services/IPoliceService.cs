using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IPoliceService
    {
        Task<PoliceRicevutaResult> GetRicevuta(string date);
        Task Test(PoliceSendRequest request);
        Task Send(PoliceSendRequest request);
    }
}
