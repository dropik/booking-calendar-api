using BookingCalendarApi.Models.DTO;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface IIstatService
    {
        Task SendNewData(IstatSendDataRequest request);
        Task<IstatLastDateResponse> GetLastDate();
        Task<IstatMovementsDTO> GetMovements();
        Task SendMovements(IstatMovementsDTO movements);
    }
}
