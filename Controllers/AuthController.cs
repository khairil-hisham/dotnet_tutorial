using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApi.Data;
using AuthApi.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username already exists.");

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = request.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return BadRequest("User not found.");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

            if (computedHash != Convert.ToBase64String(user.PasswordHash))
                return BadRequest($"Wrong password. ch = {computedHash}, ph = {user.PasswordHash}");

            Console.WriteLine($" ch = {computedHash}, ph = {Convert.ToBase64String(user.PasswordHash)}");

            string token = CreateToken(user);
            return Ok(new { token });
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
