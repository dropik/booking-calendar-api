using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IPoliceService
    {
        Task<PoliceRicevutaResponse> GetRicevuta(string date);
        Task Test(PoliceSendRequest request);
        Task Send(PoliceSendRequest request);
        Task<List<string>> GetProvinces();
    }
}
