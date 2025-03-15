using Microsoft.IdentityModel.Tokens;
using QuizApi.Data;
using QuizApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuizApi.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly JsonDataContext _dataContext;

        public AuthService(IConfiguration configuration, JsonDataContext dataContext)
        {
            _configuration = configuration;
            _dataContext = dataContext;
        }

        public User? Authenticate(string username, string password)
        {
            var users = _dataContext.GetUsers();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password); // Note: Use hashed passwords in production
            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Add user ID
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) // Add role
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}