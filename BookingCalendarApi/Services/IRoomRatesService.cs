using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface IRoomRatesService
    {
        Task<RoomRatesResponse> Get();
    }
}
