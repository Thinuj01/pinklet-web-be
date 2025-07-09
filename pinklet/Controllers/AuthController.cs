using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using pinklet.data;
using pinklet.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly EmailSettings _emailSettings;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            this.configuration = configuration;
            _emailSettings = emailSettings.Value; 
        }

        // POST: api/Auth/register
        // API for register a guest user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = HashPassword(request.Password),
                PhoneNumber = "",
                Role = "User",
                Availability = "not-verified",
                EmailVerificationToken = Guid.NewGuid().ToString(),
                TokenGeneratedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _ = SendConfirmationEmail(user);

            return Ok("User registered successfully. Please check your email to verify.");
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLoginAsync([FromBody] GoogleLoginRequest request)
        {
            // TODO: Check if user exists in DB and create user if not (you can use EF Core here)

            // For demonstration, we directly return JWT

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {

            }
            else {
                var user = new User
                {
                    FirstName = request.Name,
                    LastName = "",
                    Email = request.Email,
                    Password = "",
                    PhoneNumber = "",
                    Role = "User",
                    Availability = "verified",
                    EmailVerificationToken = "",
                    TokenGeneratedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }


                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, request.Sub),
                new Claim(JwtRegisteredClaimNames.Email, request.Email),
                new Claim("name", request.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                email = request.Email,
                name = request.Name
            });
        }


        // POST: api/Auth/fpwd
        // API for sending OTP to user's email for account recovery
        [HttpPost("fpwd")]
        public async Task<IActionResult> SendingOtpAccountRecovery([FromBody] EmailRequest request)
        {
            System.Diagnostics.Debug.WriteLine("Debug message");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("Invaild Email");
            }

            var otp = GenerateOtpCode();
            user.EmailVerificationToken = otp;
            user.TokenGeneratedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            await SendOTPEmail(user);

            return Ok("OTP sent to your email. Please check your inbox.");
        }

        // POST: api/Auth/fpwd/verify
        // API for verifying OTP for account recovery
        [HttpPost("fpwd/verify")]
        public async Task<IActionResult> VerifyOtpAccountRecovery([FromBody] AccountRecoveryRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || user.EmailVerificationToken != request.Otp)
            {
                return BadRequest("Invalid OTP.");
            }
            return Ok("OTP verified successfully. You can now reset your password.");
        }

        // POST: api/Auth/fpwd/reset
        // API for resetting password after OTP verification
        [HttpPost("fpwd/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest("Invalid email.");
            user.Password = HashPassword(request.Password);
            user.EmailVerificationToken = null; 
            user.TokenGeneratedAt = null; 
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok("Password reset successfully.");
        }

        // POST: api/Auth/login
        // API for authenticate user & generate JWT tokens
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || user.Password != HashPassword(request.Password))
                return Unauthorized("Invalid username or password");

            return Ok(new
            {
                token = GenerateJwtToken(user),
                email = user.Email,
                name = user.FirstName,
                lname = user.LastName,
            });
        }

        // GET: api/Auth/user/{id}
        // API for getting user details by ID, requires JWT authentication
        [HttpGet("user/{id}")]
        [Authorize] // Requires valid JWT
        public async Task<IActionResult> GetUserDetails(int id)
        {
            // Get user ID from JWT token
            var tokenUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (tokenUserId == null || tokenUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to access this user's data.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.Email
            });
        }

        // GET: api/Auth/verify-email?token={token}
        // API for verifying email using a token, redirects to frontend after verification
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is Missing.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

            if (user == null)
                return BadRequest("Invalid or expired token.");

            if (user.TokenGeneratedAt != null && (DateTime.UtcNow - user.TokenGeneratedAt.Value).TotalHours > 24)
                return BadRequest("Token has expired.");


            user.Availability = "verified";
            user.EmailVerificationToken = null;
            user.TokenGeneratedAt = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Email verified susccessfully");
        }


        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Generate JWT token for authenticated user
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Send confirmation email to user after registration
        private async Task SendConfirmationEmail(User user)
        {
            var confirmationLink = $"http://localhost:5173/confirm-email?token={Uri.EscapeDataString(user.EmailVerificationToken)}";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "Email Confirmation",
                Body = $"Hi {user.FirstName},<br/>Click <a href='{confirmationLink}'>here</a> to confirm your email.",
                IsBodyHtml = true
            };

            mailMessage.To.Add(user.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);
        }

        // Send OTP email for account recovery
        private async Task SendOTPEmail(User user)
        {
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "Email Verification OTP",
                Body = $"Hi {user.FirstName} <br/><br/> Your <b>One Time Password is {user.EmailVerificationToken}<b/>.<br/><br/>Please <b>do not share<b/> this with anyone.",
                IsBodyHtml = true
            };

            mailMessage.To.Add(user.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);
        }

        // Generate a random OTP code
        private string GenerateOtpCode(int length = 6)
        {
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var otp = new StringBuilder(length);
            foreach (var b in bytes)
            {
                otp.Append((b % 10).ToString());
            }
            return otp.ToString();
        }


    }

    // DTO for get loging credientails as object
    public class UserDto
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class AccountRecoveryRequest
    {
        public string Otp { get; set; }
        public string Email { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Sub { get; set; } 
        public string Picture { get; set; }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }
}
