using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwt.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(_jwt.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
            };
        }
    }
}
