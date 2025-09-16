using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.Core.Interfaces;
using Library.Web.Models;
using Library.Web.Services;
using System.Security.Claims;

namespace Library.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthenticationService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Email, request.Password);
                if (user == null)
                {
                    return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Invalid email or password"));
                }

                var token = _jwtService.GenerateToken(user);
                var response = new LoginResponse
                {
                    Token = token,
                    User = UserDto.FromUser(user)
                };

                return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LoginResponse>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register(RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(request.Name, request.Email, request.Password);
                if (user == null)
                {
                    return BadRequest(ApiResponse<UserDto>.ErrorResult("Registration failed"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResult(UserDto.FromUser(user), "Registration successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(ApiResponse<bool>.ErrorResult("Invalid user"));
                }

                var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                if (!success)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResult("Password change failed"));
                }

                return Ok(ApiResponse<bool>.SuccessResult(true, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(ApiResponse<UserDto>.ErrorResult("Invalid user"));
                }

                // For now, we'll return user info from the JWT claims
                // In a real app, you might want to fetch fresh data from the database
                var userDto = new UserDto
                {
                    Id = userId,
                    Name = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                    Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                    Role = Enum.Parse<Library.Core.Models.UserRole>(User.FindFirst(ClaimTypes.Role)?.Value ?? "User"),
                    IsEnabled = true,
                    CreatedAt = DateTime.Now // This would come from database in real scenario
                };

                return Ok(ApiResponse<UserDto>.SuccessResult(userDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }
    }
}