using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookingCalendarApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly BookingCalendarContext _context;
        private readonly JWT _jwt;

        public AuthService(BookingCalendarContext context, IOptions<JWT> jwt)
        {
            _context = context;
            _jwt = jwt.Value;
        }

        public async Task<TokenResponse?> GetToken(TokenRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<string>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(request.Username, user.PasswordHash, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return null;
            }

            return await GenerateTokens(user.Username);
        }

        public async Task<TokenResponse?> GetToken(RefreshTokenRequest request)
        {
            var key = Encoding.UTF8.GetBytes(_jwt.Key);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = _jwt.Issuer,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(request.AccessToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (string.IsNullOrWhiteSpace(principal.Identity?.Name))
            {
                return null;
            }
            var identityName = principal.Identity.Name;
            var userRefreshToken = await _context.UserRefreshTokens.SingleOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.Username == identityName);
            if (userRefreshToken == null)
            {
                return null;
            }
            if (userRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            return await GenerateTokens(identityName);
        }

        private async Task<TokenResponse> GenerateTokens(string username)
        {
            var accessToken = GenerateAccessToken(username);
            var refreshToken = await GenerateRefreshToken(username);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        private string GenerateAccessToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwt.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
            };
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(accessToken);
        }

        private async Task<string> GenerateRefreshToken(string username)
        {
            var randomNumber = new byte[128];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);

            _context.UserRefreshTokens.Add(new()
            {
                RefreshToken = token,
                Username = username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenExpirationMinutes),
            });
            await _context.SaveChangesAsync();

            return token;
        }
    }
}
