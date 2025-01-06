using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Modules.User
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        
        private readonly IUserRepository _userRepository;

        public UserController(IUserService service , IUserRepository userRepository)
        {
            _service = service;
            _userRepository = userRepository;
        }

        
        [HttpGet]
        [Authorize]
        public IActionResult DecodeUserToken()
        {
            if (User.Identity is not ClaimsIdentity claimsIdentity) return Unauthorized();
    
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString(); // Extract the token from the Authorization header
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized();
            }
            
            var token = authHeader.Substring("Bearer ".Length).Trim(); // Get the token part
            var principal = _service.DecodeToken(token); // Decode the token
            var username = principal.FindFirst(ClaimTypes.Name)?.Value; // Access user claims
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var email = GetEmailByUserId(int.Parse(userId));
            
            return Ok(new { Username = username, UserId = userId, Email = email });
        }
        private string GetEmailByUserId(int userId)
        {
            
            var user = _userRepository.FindBy
                (u => u.Id == userId).FirstOrDefault();

            if (user != null)
            {
                return user.Email; 
            }
            else
            {
                return "No email available"; 
            }
        }
        
        [HttpPut("update/{id:int}")]
        public IActionResult Update(int id, [FromBody] UserDetailResponse.UserUpdateRequest item)
        {
            _service.Update(id, item);
            return NoContent(); 
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult GenerateToken([FromBody] UserSignUpRequest item)
        {
            var token = _service.GenerateToken(item);
            return Ok(new { Token = token });
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginRequest item)
        {
            var token = _service.Login(item);
            return Ok(new { Token = token });
        }
    }
}