using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IIstatService
    {
        public Task SendNewData(IstatSendDataRequest request);
        public Task<IstatLastDateResponse> GetLastDate();
    }
}
