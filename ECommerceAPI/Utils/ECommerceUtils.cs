// ============================================================
// UTILS - Password Helper & JWT Helper
// ============================================================

using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ShopWaveBlazor.Web.ECommerceAPI.Utils
{
    // ── PASSWORD HELPER ─────────────────────────────────────
    public static class PasswordHelper
    {
        /// <summary>Hash password using SHA-256 with salt</summary>
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // Combine password + salt and hash
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combined      = salt.Concat(passwordBytes).ToArray();

            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(combined);

            // Store salt + hash together (Base64 encoded)
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        /// <summary>Verify plain-text password against stored hash</summary>
        public static bool VerifyPassword(string plainPassword, string storedHash)
        {
            try
            {
                string[] parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                byte[] salt          = Convert.FromBase64String(parts[0]);
                byte[] expectedHash  = Convert.FromBase64String(parts[1]);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] combined      = salt.Concat(passwordBytes).ToArray();

                using var sha256 = SHA256.Create();
                byte[] actualHash = sha256.ComputeHash(combined);

                return expectedHash.SequenceEqual(actualHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if stored hash is the old plain SHA-256 format (from SQL seed data).
        /// Seed data uses: SHA256("admin123") without salt.
        /// </summary>
        public static bool VerifyLegacyPassword(string plainPassword, string storedHash)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            string hash  = Convert.ToHexString(bytes).ToLower();
            return hash == storedHash.ToLower();
        }
    }

    // ── JWT HELPER ──────────────────────────────────────────
    public static class JwtHelper
    {
        /// <summary>Generate a JWT token for authenticated user</summary>
        public static string GenerateToken(int userID, string username, string userType,
                                           string secretKey, int expiryMinutes = 480)
        {
            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userID.ToString()),
                new Claim(ClaimTypes.Name,           username),
                new Claim(ClaimTypes.Role,           userType),
                new Claim("UserID",                  userID.ToString()),
                new Claim("UserType",                userType)
            };

            var token = new JwtSecurityToken(
                issuer:   "ECommerceAPI",
                audience: "ECommerceFrontend",
                claims:   claims,
                expires:  DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>Get UserID from HttpContext claims</summary>
        public static int GetUserID(ClaimsPrincipal user)
        {
            var claim = user.FindFirst("UserID") ?? user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        /// <summary>Get UserType from HttpContext claims</summary>
        public static string GetUserType(ClaimsPrincipal user)
        {
            return user.FindFirst("UserType")?.Value
                ?? user.FindFirst(ClaimTypes.Role)?.Value
                ?? "Customer";
        }
    }

    // ── VALIDATION HELPER ───────────────────────────────────
    public static class ValidationHelper
    {
        public static bool IsValidPakistaniPhone(string phone)
        {
            // Accepts: 03001234567, +923001234567, 923001234567
            var cleaned = phone.Replace(" ", "").Replace("-", "");
            return System.Text.RegularExpressions.Regex.IsMatch(
                cleaned, @"^(\+92|92|0)3[0-9]{9}$");
        }

        public static bool IsValidEmail(string email)
            => System.Text.RegularExpressions.Regex.IsMatch(
                email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public static bool IsValidCreditCard(string cardNumber)
            => System.Text.RegularExpressions.Regex.IsMatch(
                cardNumber.Replace(" ", ""), @"^\d{16}$");

        public static bool IsValidCVV(string cvv)
            => System.Text.RegularExpressions.Regex.IsMatch(cvv, @"^\d{3}$");

        public static bool IsValidExpiry(string expiry)
        {
            // Format: MM/YY or MM/YYYY
            if (!System.Text.RegularExpressions.Regex.IsMatch(expiry, @"^\d{2}/\d{2,4}$"))
                return false;

            var parts = expiry.Split('/');
            int month = int.Parse(parts[0]);
            int year  = int.Parse(parts[1]);

            if (year < 100) year += 2000;
            var expiryDate = new DateTime(year, month,
                DateTime.DaysInMonth(year, month));

            return month is >= 1 and <= 12 && expiryDate >= DateTime.Now;
        }

        /// <summary>Generate a unique transaction ID</summary>
        public static string GenerateTransactionID(string prefix)
        {
            return $"TXN-{prefix}-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }
    }
}
