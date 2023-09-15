using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace BookingCalendarApi.NETFramework.Filters
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
            {
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new UnauthorizedResult(new List<AuthenticationHeaderValue>() { new AuthenticationHeaderValue("Bearer") }, context.Request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
            {
                context.ErrorResult = new UnauthorizedResult(new List<AuthenticationHeaderValue>() { new AuthenticationHeaderValue("Bearer") }, context.Request);
                return;
            }
            
            context.Principal = principal;
        }

        private Task<ClaimsPrincipal> AuthenticateJwtToken(string token)
        {
            if (ValidateToken(token, out var claims))
            {
                var identity = new ClaimsIdentity(claims, "Jwt");
                var user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<ClaimsPrincipal>(null);
        }

        private static bool ValidateToken(string token, out List<Claim> claims)
        {
            claims = new List<Claim>();

            var simplePrinciple = GetPrincipal(token);
            if (!(simplePrinciple?.Identity is ClaimsIdentity identity))
            {
                return false;
            }

            if (!identity.IsAuthenticated)
            {
                return false;
            }

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var role = identity.FindFirst(ClaimTypes.Role)?.Value ?? "";
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim(ClaimTypes.Role, role));

            return true;
        }

        private static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!(tokenHandler.ReadToken(token) is JwtSecurityToken jwtToken))
                {
                    return null;
                }

                var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["JWT_Key"]));

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = symmetricKey,
                    ValidIssuer = ConfigurationManager.AppSettings["JWT_Issuer"],
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}