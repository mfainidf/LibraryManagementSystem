using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Library.API.Configuration;
using Library.API.Models;
using Library.Core.Interfaces;
using Library.Core.Models;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthenticationService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Email, request.Password);
                var token = GenerateJwtToken(user);

                return Ok(new TokenResponse { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new MessageResponse { Message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(request.Name, request.Email, request.Password);
                var token = GenerateJwtToken(user);

                return Ok(new TokenResponse { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAdminAsync(request.Name, request.Email, request.Password);
                var token = GenerateJwtToken(user);

                return Ok(new TokenResponse { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                    throw new InvalidOperationException("User ID not found in token"));

                await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                return Ok(new MessageResponse { Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageResponse { Message = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured");
            var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
            var expirationHoursStr = jwtSection["ExpirationHours"] ?? "24";
            var expirationHours = double.Parse(expirationHoursStr);

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(expirationHours),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class ChangePasswordRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
