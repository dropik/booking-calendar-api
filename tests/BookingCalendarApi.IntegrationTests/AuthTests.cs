using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Responses;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BookingCalendarApi.IntegrationTests
{
    public class AuthTests : ApiTest
    {
        public AuthTests(ApiFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Master_user_seeded_by_the_migrations_can_log_in()
        {
            var tokens = await LoginAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokens.AccessToken);
            token.Issuer.Should().Be(ApiFixture.JwtIssuer);
            token.Claims.Should().Contain(c => c.Type == JWT.STRUCTURE_CLAIM && c.Value == "-1");
            token.Claims.Should().Contain(c => c.Type == "role" && c.Value == "admin");

            await using var context = Fixture.CreateContext();
            (await context.UserRefreshTokens.SingleAsync()).RefreshToken.Should().Be(tokens.RefreshToken);
        }

        [Theory]
        [InlineData(ApiFixture.MasterUsername, "wrong")]
        [InlineData("nobody@hotel.it", ApiFixture.MasterPassword)]
        public async Task Wrong_credentials_are_unauthorized(string username, string password)
        {
            var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/token", new { username, password });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_returns_a_new_working_pair_of_tokens()
        {
            var tokens = await LoginAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);

            var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/refresh", tokens);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var refreshed = (await response.Content.ReadFromJsonAsync<TokenResponse>(Json))!;
            refreshed.RefreshToken.Should().NotBe(tokens.RefreshToken);
            var client = Anonymous();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
            (await client.GetAsync("/api/v1/users/current")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Refresh_with_a_wrong_refresh_token_is_unauthorized()
        {
            var tokens = await LoginAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);

            var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/refresh", new { tokens.AccessToken, RefreshToken = "forged" });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_with_a_malformed_access_token_is_unauthorized()
        {
            var tokens = await LoginAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);

            var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/refresh", new { AccessToken = "not-a-jwt", tokens.RefreshToken });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("GET", "/api/v1/users/current")]
        [InlineData("GET", "/api/v1/floors")]
        [InlineData("GET", "/api/v1/rooms")]
        [InlineData("POST", "/api/v1/assignments")]
        [InlineData("GET", "/api/v1/bookings?from=2023-06-01&to=2023-06-02")]
        [InlineData("GET", "/api/v1/clients/by-tile?bookingId=1&tileId=1-20230601-20230602")]
        [InlineData("GET", "/api/v1/city-tax?from=2023-06-01&to=2023-06-02")]
        [InlineData("GET", "/api/v1/istat/countries")]
        [InlineData("GET", "/api/v1/police/provinces")]
        [InlineData("GET", "/api/v1/structures/current/api-keys")]
        public async Task Protected_endpoints_require_a_token(string method, string url)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), url) { Content = method == "POST" ? JsonContent.Create(new { }) : null };

            (await Anonymous().SendAsync(request)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_with_a_forged_access_token_is_unauthorized()
        {
            var tokens = await LoginAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);
            var handler = new JwtSecurityTokenHandler();
            var forged = handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = ApiFixture.JwtIssuer,
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, ApiFixture.MasterUsername), new Claim(JWT.REFRESH_TOKEN_CLAIM, "1") }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("some-other-signing-key-that-is-long-enough!")), SecurityAlgorithms.HmacSha256),
            }));

            var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/refresh", new { AccessToken = forged, tokens.RefreshToken });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Tokens_signed_with_another_key_are_rejected()
        {
            var handler = new JwtSecurityTokenHandler();
            var forged = handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = ApiFixture.JwtIssuer,
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, ApiFixture.MasterUsername), new Claim(JWT.STRUCTURE_CLAIM, "-1") }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("some-other-signing-key-that-is-long-enough!")), SecurityAlgorithms.HmacSha256),
            }));
            var client = Anonymous();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", forged);

            (await client.GetAsync("/api/v1/users/current")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Tokens_of_another_issuer_are_rejected()
        {
            var handler = new JwtSecurityTokenHandler();
            var foreign = handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = "someone-else",
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, ApiFixture.MasterUsername), new Claim(JWT.STRUCTURE_CLAIM, "-1") }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFixture.JwtKey)), SecurityAlgorithms.HmacSha256),
            }));
            var client = Anonymous();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", foreign);

            (await client.GetAsync("/api/v1/users/current")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Cors_allows_the_frontend_only()
        {
            var preflight = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/token");
            preflight.Headers.Add("Origin", "http://frontend.test");
            preflight.Headers.Add("Access-Control-Request-Method", "POST");
            preflight.Headers.Add("Access-Control-Request-Headers", "content-type");
            var foreign = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/token");
            foreign.Headers.Add("Origin", "http://evil.test");
            foreign.Headers.Add("Access-Control-Request-Method", "POST");

            var allowed = await Anonymous().SendAsync(preflight);
            var refused = await Anonymous().SendAsync(foreign);

            allowed.Headers.GetValues("Access-Control-Allow-Origin").Should().Equal("http://frontend.test");
            refused.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        }

        [Fact]
        public async Task Expired_tokens_are_rejected_with_a_header_telling_so()
        {
            var handler = new JwtSecurityTokenHandler();
            var expired = handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = ApiFixture.JwtIssuer,
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, ApiFixture.MasterUsername), new Claim(JWT.STRUCTURE_CLAIM, "-1") }),
                NotBefore = DateTime.UtcNow.AddMinutes(-30),
                IssuedAt = DateTime.UtcNow.AddMinutes(-30),
                Expires = DateTime.UtcNow.AddMinutes(-10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFixture.JwtKey)), SecurityAlgorithms.HmacSha256),
            }));
            var client = Anonymous();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expired);

            var response = await client.GetAsync("/api/v1/users/current");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Headers.GetValues("IS-TOKEN-EXPIRED").Should().Equal("true");
        }
    }
}
