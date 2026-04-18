using Microsoft.AspNetCore.Mvc;
using Saferide.Services;

namespace Saferide.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Authentication _authentication;

        public AuthController(Authentication authentication)
        {
            _authentication = authentication;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var user = _authentication.Register(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.Role
            );

            if (user == null)
            {
                return BadRequest("Registration failed. Email may already exist or role is invalid.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var session = _authentication.Login(request.Email, request.Password);
            
            if (session == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new 
            {
                sessionId = session.GetSessionId()
            });
        }

        [HttpPost("logout")] // Needs to be updated
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            bool success = _authentication.Logout(request.SessionId);

            if (!success)
            {
                return BadRequest("Logout failed. Invalid session ID.");
            }

            return Ok("Logout successful.");
        }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LogoutRequest // Not needed after front end stores the sessionId and return it correctly
    {
        public string SessionId { get; set; } = "";
    }
}