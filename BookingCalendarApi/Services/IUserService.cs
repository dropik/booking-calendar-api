using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface IUserService
    {
        Task<CurrentUserResponse> GetCurrentUser();
    }
}
