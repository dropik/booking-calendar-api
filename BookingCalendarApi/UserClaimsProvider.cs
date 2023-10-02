using BookingCalendarApi.Services;
using System.Security.Claims;

namespace BookingCalendarApi
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserClaimsProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ClaimsPrincipal User => _contextAccessor.HttpContext!.User;
    }
}
