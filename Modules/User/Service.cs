using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core;

namespace WebApplication1.Modules.User
{
    public interface IUserService
    {
        ClaimsPrincipal DecodeToken(string token);
        void Update(int id, UserDetailResponse.UserUpdateRequest request);
        string GenerateToken(UserSignUpRequest request);
        string Login(UserLoginRequest request);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
        

        public void Update(int id, UserDetailResponse.UserUpdateRequest request)
        {
            var userToUpdate = _repository.FindBy(e => e.Id == id).FirstOrDefault();
            if (userToUpdate == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            _mapper.Map(request, userToUpdate);
            _repository.Update(userToUpdate);
            _repository.Commit();
        }

        public string GenerateToken(UserSignUpRequest request)
        {
            // Check if the username already exists
            var existingUser = _repository.FindBy(e => e.Username == request.Username).FirstOrDefault();
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            // Check if the email already exists
            var existingEmail = _repository.FindBy(e => e.Email == request.Email).FirstOrDefault();
            if (existingEmail != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // Validate the email format (for example, check for a Gmail email format)
            if (!IsValidEmail(request.Email))
            {
                throw new InvalidOperationException("Invalid email format. Please provide a valid email address.");
            }

            // Create the user entity and save to repository
            var user = _mapper.Map<UserEntity>(request);
            _repository.Add(user);
            _repository.Commit();

            // Get the JWT key and other configurations
            var jwtKey = AppEnvironment.JwtKey;
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            int userId = user.Id;
            // Define the claims for the JWT token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("UserId", userId.ToString())
            };

            // Create the JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: AppEnvironment.JwtIssuer,
                audience: AppEnvironment.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            // Return the JWT token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsValidEmail(string email)
        {
            // Simple email format validation for '@gmail.com'
            return email.Contains("@gmail.com") && email.Length > 10;
        }


        public string Login(UserLoginRequest request)
        {
            var user = _repository.FindBy(e => e.Username == request.Username).FirstOrDefault();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            var jwtKey = AppEnvironment.JwtKey;
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            int userId = user.Id;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Convert to string for claims
                new Claim("UserId", userId.ToString()) // Optional: add UserId as a custom claim
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: AppEnvironment.JwtIssuer,
                audience: AppEnvironment.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}