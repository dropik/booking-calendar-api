using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookingCalendarApi.UnitTests.Services
{
    public class AuthServiceTests
    {
        private const string Key = "unit-test-signing-key-which-is-long-enough-for-hmac-sha-512-too!!";
        private const string Issuer = "unit-test-issuer";

        private readonly InMemoryRepository _repository = new();
        private readonly JWT _jwt = new() { Key = Key, Issuer = Issuer, AccessTokenExpirationMinutes = 10, RefreshTokenExpirationMinutes = 60 };
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _service = new AuthService(_repository, Options.Create(_jwt));
            SeedUser("mario@hotel.it", "secret", structureId: 7, isAdmin: false);
            SeedUser("admin@hotel.it", "admin-secret", structureId: 8, isAdmin: true);
        }

        private void SeedUser(string username, string password, long structureId, bool isAdmin)
        {
            _repository.Seed(new User
            {
                Username = username,
                PasswordHash = new PasswordHasher<string>().HashPassword(username, password),
                StructureId = structureId,
                IsAdmin = isAdmin,
            });
        }

        private static ClaimsPrincipal Validate(string accessToken, out JwtSecurityToken token)
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidIssuer = Issuer,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
            }, out var securityToken);
            token = (JwtSecurityToken)securityToken;
            return principal;
        }

        private static string AccessToken(
            string? username, string refreshTokenId, string key = Key, string issuer = Issuer,
            string algorithm = SecurityAlgorithms.HmacSha256, DateTime? expires = null)
        {
            var claims = new List<Claim> { new(JWT.REFRESH_TOKEN_CLAIM, refreshTokenId) };
            if (username != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, username));
            }
            var expiresAt = expires ?? DateTime.UtcNow.AddMinutes(5);
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = new ClaimsIdentity(claims),
                NotBefore = expiresAt.AddMinutes(-10),
                IssuedAt = expiresAt.AddMinutes(-10),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), algorithm),
            }));
        }

        private UserRefreshToken SeedRefreshToken(string username = "mario@hotel.it", string token = "refresh-token", DateTime? expiresAt = null)
        {
            return _repository.Seed(new UserRefreshToken
            {
                Username = username,
                RefreshToken = token,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddMinutes(30),
            });
        }

        [Fact]
        public async Task Login_issues_a_signed_access_token_with_user_claims()
        {
            var response = await _service.GetToken(new TokenRequest { Username = "mario@hotel.it", Password = "secret" });

            var principal = Validate(response.AccessToken, out var token);
            var storedRefreshToken = _repository.Stored<UserRefreshToken>().Single();

            principal.Identity!.Name.Should().Be("mario@hotel.it");
            principal.FindFirst(JWT.STRUCTURE_CLAIM)!.Value.Should().Be("7");
            principal.FindFirst(JWT.REFRESH_TOKEN_CLAIM)!.Value.Should().Be(storedRefreshToken.Id.ToString());
            principal.IsInRole(User.ADMIN_ROLE).Should().BeFalse();
            token.Issuer.Should().Be(Issuer);
            token.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
            token.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(10), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task Login_as_admin_adds_the_admin_role()
        {
            var response = await _service.GetToken(new TokenRequest { Username = "admin@hotel.it", Password = "admin-secret" });

            var principal = Validate(response.AccessToken, out _);

            principal.IsInRole(User.ADMIN_ROLE).Should().BeTrue();
            principal.FindFirst(JWT.STRUCTURE_CLAIM)!.Value.Should().Be("8");
        }

        [Fact]
        public async Task Login_stores_a_random_refresh_token_for_the_user()
        {
            var first = await _service.GetToken(new TokenRequest { Username = "mario@hotel.it", Password = "secret" });
            var second = await _service.GetToken(new TokenRequest { Username = "mario@hotel.it", Password = "secret" });

            var stored = _repository.Stored<UserRefreshToken>();
            stored.Select(t => t.RefreshToken).Should().Equal(first.RefreshToken, second.RefreshToken);
            stored.Should().OnlyContain(t => t.Username == "mario@hotel.it");
            stored.Should().OnlyContain(t => t.ExpiresAt > DateTime.UtcNow.AddMinutes(59) && t.ExpiresAt < DateTime.UtcNow.AddMinutes(61));
            Convert.FromBase64String(first.RefreshToken).Should().HaveCount(128);
            first.RefreshToken.Should().NotBe(second.RefreshToken);
        }

        [Theory]
        [InlineData("mario@hotel.it", "wrong")]
        [InlineData("nobody@hotel.it", "secret")]
        public async Task Login_with_wrong_credentials_returns_null_and_stores_nothing(string username, string password)
        {
            var response = await _service.GetToken(new TokenRequest { Username = username, Password = password });

            response.Should().BeNull();
            _repository.Stored<UserRefreshToken>().Should().BeEmpty();
        }

        [Fact]
        public async Task Refresh_issues_new_tokens_even_for_an_expired_access_token()
        {
            var refreshToken = SeedRefreshToken();
            var expiredAccessToken = AccessToken("mario@hotel.it", refreshToken.Id.ToString(), expires: DateTime.UtcNow.AddMinutes(-5));

            var response = await _service.GetToken(new RefreshTokenRequest { AccessToken = expiredAccessToken, RefreshToken = "refresh-token" });

            var principal = Validate(response.AccessToken, out _);
            principal.Identity!.Name.Should().Be("mario@hotel.it");
            principal.FindFirst(JWT.STRUCTURE_CLAIM)!.Value.Should().Be("7");
            response.RefreshToken.Should().NotBe("refresh-token");
            _repository.Stored<UserRefreshToken>().Select(t => t.RefreshToken).Should().Contain(response.RefreshToken);
        }

        [Fact]
        public async Task Refresh_rejects_tokens_signed_with_another_key()
        {
            var refreshToken = SeedRefreshToken();
            var forged = AccessToken("mario@hotel.it", refreshToken.Id.ToString(), key: "another-signing-key-which-is-also-long-enough-for-hmac!!");

            var act = () => _service.GetToken(new RefreshTokenRequest { AccessToken = forged, RefreshToken = "refresh-token" });

            await act.Should().ThrowAsync<SecurityTokenInvalidSignatureException>();
        }

        [Fact]
        public async Task Refresh_rejects_tokens_from_another_issuer()
        {
            var refreshToken = SeedRefreshToken();
            var foreign = AccessToken("mario@hotel.it", refreshToken.Id.ToString(), issuer: "someone-else");

            var act = () => _service.GetToken(new RefreshTokenRequest { AccessToken = foreign, RefreshToken = "refresh-token" });

            await act.Should().ThrowAsync<SecurityTokenInvalidIssuerException>();
        }

        [Fact]
        public async Task Refresh_rejects_tokens_signed_with_another_algorithm()
        {
            var refreshToken = SeedRefreshToken();
            var otherAlgorithm = AccessToken("mario@hotel.it", refreshToken.Id.ToString(), algorithm: SecurityAlgorithms.HmacSha512);

            var act = () => _service.GetToken(new RefreshTokenRequest { AccessToken = otherAlgorithm, RefreshToken = "refresh-token" });

            await act.Should().ThrowAsync<SecurityTokenException>().WithMessage("Invalid token");
        }

        [Fact]
        public async Task Refresh_with_another_refresh_token_returns_null()
        {
            var refreshToken = SeedRefreshToken();

            var response = await _service.GetToken(new RefreshTokenRequest
            {
                AccessToken = AccessToken("mario@hotel.it", refreshToken.Id.ToString()),
                RefreshToken = "another-refresh-token",
            });

            response.Should().BeNull();
        }

        [Fact]
        public async Task Refresh_with_a_refresh_token_of_another_user_returns_null()
        {
            var refreshToken = SeedRefreshToken(username: "admin@hotel.it");

            var response = await _service.GetToken(new RefreshTokenRequest
            {
                AccessToken = AccessToken("mario@hotel.it", refreshToken.Id.ToString()),
                RefreshToken = "refresh-token",
            });

            response.Should().BeNull();
        }

        [Fact]
        public async Task Refresh_with_an_expired_refresh_token_returns_null()
        {
            var refreshToken = SeedRefreshToken(expiresAt: DateTime.UtcNow.AddMinutes(-1));

            var response = await _service.GetToken(new RefreshTokenRequest
            {
                AccessToken = AccessToken("mario@hotel.it", refreshToken.Id.ToString()),
                RefreshToken = "refresh-token",
            });

            response.Should().BeNull();
        }

        [Theory]
        [InlineData("999")]
        [InlineData("not-a-number")]
        public async Task Refresh_with_an_unknown_refresh_token_id_returns_null(string refreshTokenId)
        {
            SeedRefreshToken();

            var response = await _service.GetToken(new RefreshTokenRequest
            {
                AccessToken = AccessToken("mario@hotel.it", refreshTokenId),
                RefreshToken = "refresh-token",
            });

            response.Should().BeNull();
        }

        [Fact]
        public async Task Refresh_of_an_access_token_without_user_name_returns_null()
        {
            var refreshToken = SeedRefreshToken();

            var response = await _service.GetToken(new RefreshTokenRequest
            {
                AccessToken = AccessToken(null, refreshToken.Id.ToString()),
                RefreshToken = "refresh-token",
            });

            response.Should().BeNull();
        }
    }
}
