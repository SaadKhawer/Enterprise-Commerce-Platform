// ============================================================
// AUTH CONTROLLER
// Endpoints: Login, Register, Logout, Check Session
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Data;
using ShopWaveBlazor.Web.ECommerceAPI.DTOs;
using ShopWaveBlazor.Web.ECommerceAPI.Models;
using ShopWaveBlazor.Web.ECommerceAPI.Utils;

namespace ShopWaveBlazor.Web.ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ECommerceDbContext _db;
        private readonly IConfiguration    _config;

        public AuthController(ECommerceDbContext db, IConfiguration config)
        {
            _db     = db;
            _config = config;
        }

        // ── POST /api/auth/login ────────────────────────────
        /// <summary>Authenticate user and return JWT token</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
            [FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse<LoginResponse>.Fail("Username and password are required."));

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user == null)
                return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid username or password."));

            // Try salted hash first, then legacy plain SHA-256 (seed data)
            bool valid = PasswordHelper.VerifyPassword(request.Password, user.Password)
                      || PasswordHelper.VerifyLegacyPassword(request.Password, user.Password);

            if (!valid)
                return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid username or password."));

            string secretKey = _config["Jwt:SecretKey"] ?? "ECommerceDefaultKey123!@#SuperSecure";
            string token     = JwtHelper.GenerateToken(user.UserID, user.Username,
                                                       user.UserType, secretKey);

            var response = new LoginResponse
            {
                Success  = true,
                Message  = "Login successful.",
                UserID   = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                UserType = user.UserType,
                Token    = token
            };

            return Ok(ApiResponse<LoginResponse>.Ok(response, "Login successful."));
        }

        // ── POST /api/auth/register ─────────────────────────
        /// <summary>Register a new customer account</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> Register(
            [FromBody] RegisterRequest request)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
                return BadRequest(ApiResponse<string>.Fail("Username must be at least 3 characters."));

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                return BadRequest(ApiResponse<string>.Fail("Password must be at least 6 characters."));

            if (!ValidationHelper.IsValidEmail(request.Email))
                return BadRequest(ApiResponse<string>.Fail("Invalid email address."));

            if (!string.IsNullOrEmpty(request.PhoneNumber) &&
                !ValidationHelper.IsValidPakistaniPhone(request.PhoneNumber))
                return BadRequest(ApiResponse<string>.Fail(
                    "Invalid phone number. Use Pakistani format e.g. 03001234567."));

            // Check duplicates
            bool usernameTaken = await _db.Users.AnyAsync(u => u.Username == request.Username);
            if (usernameTaken)
                return Conflict(ApiResponse<string>.Fail("Username is already taken."));

            bool emailTaken = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (emailTaken)
                return Conflict(ApiResponse<string>.Fail("Email is already registered."));

            var user = new User
            {
                Username    = request.Username.Trim(),
                Password    = PasswordHelper.HashPassword(request.Password),
                Email       = request.Email.Trim().ToLower(),
                FullName    = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                Address     = request.Address?.Trim(),
                UserType    = "Customer",
                IsActive    = true,
                CreatedDate = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Account created successfully."));
        }

        // ── GET /api/auth/me ────────────────────────────────
        /// <summary>Get current logged-in user info (requires token)</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetCurrentUser()
        {
            int userID = JwtHelper.GetUserID(User);
            var user   = await _db.Users.FindAsync(userID);

            if (user == null || !user.IsActive)
                return NotFound(ApiResponse<object>.Fail("User not found."));

            return Ok(ApiResponse<object>.Ok(new
            {
                user.UserID,
                user.Username,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.UserType,
                user.CreatedDate
            }));
        }

        // ── PUT /api/auth/change-password ───────────────────
        [HttpPut("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> ChangePassword(
            [FromBody] ChangePasswordRequest request)
        {
            int userID = JwtHelper.GetUserID(User);
            var user   = await _db.Users.FindAsync(userID);

            if (user == null)
                return NotFound(ApiResponse<string>.Fail("User not found."));

            bool valid = PasswordHelper.VerifyPassword(request.OldPassword, user.Password)
                      || PasswordHelper.VerifyLegacyPassword(request.OldPassword, user.Password);

            if (!valid)
                return BadRequest(ApiResponse<string>.Fail("Current password is incorrect."));

            if (request.NewPassword.Length < 6)
                return BadRequest(ApiResponse<string>.Fail("New password must be at least 6 characters."));

            user.Password = PasswordHelper.HashPassword(request.NewPassword);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Password changed successfully."));
        }
    }

    // Simple DTO for change password (only used here)
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
