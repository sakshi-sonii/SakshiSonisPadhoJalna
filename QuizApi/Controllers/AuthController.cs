using Microsoft.AspNetCore.Mvc;
using QuizApi.Models;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _authService.Authenticate(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token, user });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}