using BookingCalendarApi.Services;
using System.Security.Claims;
using System.Web;

namespace BookingCalendarApi.NETFramework
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        public ClaimsPrincipal User => HttpContext.Current?.User as ClaimsPrincipal;
    }
}
