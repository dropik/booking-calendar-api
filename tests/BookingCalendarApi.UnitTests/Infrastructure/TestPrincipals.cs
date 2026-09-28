using System.Security.Claims;

using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Services;

namespace BookingCalendarApi.UnitTests.Infrastructure
{
    public static class TestPrincipals
    {
        public static ClaimsPrincipal For(string username, long structureId)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JWT.STRUCTURE_CLAIM, structureId.ToString()),
            }, "Test");
            return new ClaimsPrincipal(identity);
        }

        public static IUserClaimsProvider Provider(string username, long structureId)
        {
            var provider = Substitute.For<IUserClaimsProvider>();
            provider.User.Returns(For(username, structureId));
            return provider;
        }
    }
}
