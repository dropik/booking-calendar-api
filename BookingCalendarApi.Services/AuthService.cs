using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository _repository;
        private readonly JWT _jwt;

        private const string REFRESH_TOKEN_CLAIM = "RTId";

        public AuthService(IRepository repository, IOptions<JWT> jwt)
        {
            _repository = repository;
            _jwt = jwt.Value;
        }

        public async Task<TokenResponse> GetToken(TokenRequest request)
        {
            var user = await _repository.SingleOrDefaultAsync( _repository.Users.Where(u => u.Username == request.Username));
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

        public async Task<TokenResponse> GetToken(RefreshTokenRequest request)
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
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (string.IsNullOrWhiteSpace(principal.Identity?.Name))
            {
                return null;
            }
            var identityName = principal.Identity.Name;
            long refreshTokenId = 0;
            try
            {
                refreshTokenId = long.Parse(principal.Claims.FirstOrDefault(c => c.Type == REFRESH_TOKEN_CLAIM)?.Value ?? "0");
            }
            catch (Exception)
            {
                return null;
            }
            var userRefreshToken = await _repository.SingleOrDefaultAsync(_repository.UserRefreshTokens.Where(u => u.Id == refreshTokenId));
            if (userRefreshToken == null
                || userRefreshToken.RefreshToken != request.RefreshToken
                || userRefreshToken.Username != identityName
                || userRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            return await GenerateTokens(identityName);
        }

        private async Task<TokenResponse> GenerateTokens(string username)
        {
            var refreshToken = await GenerateRefreshToken(username);
            var accessToken = GenerateAccessToken(username, refreshToken);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken,
            };
        }

        private string GenerateAccessToken(string username, UserRefreshToken userRefreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwt.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(REFRESH_TOKEN_CLAIM, userRefreshToken.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
            };
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(accessToken);
        }

        private async Task<UserRefreshToken> GenerateRefreshToken(string username)
        {
            var randomNumber = new byte[128];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);

                var result = _repository.Add(new UserRefreshToken()
                {
                    RefreshToken = token,
                    Username = username,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenExpirationMinutes),
                });
                await _repository.SaveChangesAsync();
                return result;
            }
        }
    }
}
