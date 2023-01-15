using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface IIstatService
    {
        public Task SendNewData(IstatSendDataRequest request);
        public Task<IstatLastDateResponse> GetLastDate();
    }
}
