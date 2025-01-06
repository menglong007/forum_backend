using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Core
{
    public class Authorization
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public Authorization(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        // Validate the token and return the claims
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            SecurityToken validatedToken;

            // Validate the token and extract claims
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = AppEnvironment.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = AppEnvironment.JwtAudience,
                ValidateLifetime = true,
                // You can set ClockSkew here if needed
            }, out validatedToken);

            return principal; // Return the ClaimsPrincipal
        }

        // Check if the user is in a specific role
        public bool IsUserInRole(string role)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user != null && user.IsInRole(role);
        }

        // Optionally, you can create more methods to handle authorization logic
        public bool HasClaim(string claimType, string claimValue)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user != null && user.HasClaim(c => c.Type == claimType && c.Value == claimValue);
        }
    }
}
