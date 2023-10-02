using System.Security.Claims;

namespace BookingCalendarApi.Services
{
    public interface IUserClaimsProvider
    {
        ClaimsPrincipal User { get; }
    }
}
