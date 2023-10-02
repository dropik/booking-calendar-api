using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> GetToken(TokenRequest request);
        Task<TokenResponse> GetToken(RefreshTokenRequest request);
    }
}
