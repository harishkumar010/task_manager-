using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var existingUsers = await _userRepository.FindAsync(u => u.Username == request.Username);
            if (existingUsers.Any())
            {
                return BadRequest("Username already exists.");
            }

            CreatePasswordHash(request.Password, out string passwordHash);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var users = await _userRepository.FindAsync(u => u.Username == request.Username);
            var user = users.FirstOrDefault();

            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash))
            {
                return BadRequest("Invalid username or password.");
            }

            var token = CreateToken(user);
            return Ok(new AuthResponseDto { Token = token });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        // Extremely simple hashing for demonstration/interview purposes
        // In reality, use BCrypt or Identity
        private void CreatePasswordHash(string password, out string passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                var salt = Convert.ToBase64String(hmac.Key);
                var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
                passwordHash = $"{salt}:{hash}";
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return hash == computedHash;
            }
        }
    }
}
