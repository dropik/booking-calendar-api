using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface IAuthService
    {
        Task<TokenResponse?> GetToken(TokenRequest request);
    }
}
