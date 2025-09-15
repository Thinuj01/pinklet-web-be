using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pinklet.data;
using pinklet.Models;
using System.Net;
using System.Net.Mail;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CakeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly CloudinaryService _cloudinaryService;
        private readonly EmailSettings _emailSettings;

        public CakeController(ApplicationDbContext context, IConfiguration configuration, CloudinaryService cloudinaryService, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            this.configuration = configuration;
            _cloudinaryService = cloudinaryService;
            _emailSettings = emailSettings.Value;
        }

        // POST: api/cake
        [HttpPost]
        public async Task<IActionResult> AddCake([FromBody] Cake cake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (cake == null)
            {
                return BadRequest("Cake data is required.");
            }
            _context.Cakes.Add(cake);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cake added successfully", cake.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCakes()
        {
            var cakes = await _context.Cakes.ToListAsync();
            if (cakes == null || !cakes.Any())
            {
                return NotFound("No cakes found.");
            }
            return Ok(cakes);
        }

        [HttpPost("custom/add")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> AddCustomCake([FromForm] CustomCakeDto dto)
        {
            if (dto == null) return BadRequest("Custom cake data is required.");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return NotFound("User not found.");

            var uploadedUrls = new List<string>();
            foreach (var file in dto.CakeImages.Where(f => f != null))
            {
                var url = await _cloudinaryService.UploadImageAsync(file);
                uploadedUrls.Add(url);
            }

            var customCake = new CustomCake
            {
                UserId = dto.UserId,
                Description = dto.Description,
                CakeWeight = dto.CakeWeight,
                CakeImageLink1 = uploadedUrls.ElementAtOrDefault(0),
                CakeImageLink2 = uploadedUrls.ElementAtOrDefault(1),
                CakeImageLink3 = uploadedUrls.ElementAtOrDefault(2),
                CakeImageLink4 = uploadedUrls.ElementAtOrDefault(3),
                CakeImageLink5 = uploadedUrls.ElementAtOrDefault(4),
                CakeCode = GenerateCakeCode()
            };

            _context.CustomCakes.Add(customCake);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Custom cake added successfully", customCake.Id });
        }

        // GET: api/cake/requests
        [HttpGet("requests")]
        [Authorize]
        public async Task<IActionResult> GetCustomCakeRequests()
        {
            var requests = await _context.CustomCakes
                .Where(c => c.CakePrice == null)
                .Include(c => c.User) 
                .Select(c => new
                {
                    c.Id,
                    c.CakeCode,
                    c.Description,
                    c.CakeWeight,
                    c.CakeImageLink1,
                    c.CakeImageLink2,
                    c.CakeImageLink3,
                    c.CakeImageLink4,
                    c.CakeImageLink5,
                    UserEmail=c.User.Email,
                    UserFirstName = c.User.FirstName,
                    UserLastName = c.User.LastName,
                })
                .ToListAsync();

            if (!requests.Any())
            {
                return NotFound("No custom cake requests found.");
            }

            return Ok(requests);
        }

        // PUT: api/cake/{id}/setprice
        [HttpPut("{id}/setprice")]
        public async Task<IActionResult> SetCakePrice(int id, [FromBody] SetPriceDto dto)
        {
            var cake = await _context.CustomCakes
                .Include(c =>c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cake == null)
            {
                return NotFound("Cake not found.");
            }

            cake.CakePrice = dto.CakePrice;
            await _context.SaveChangesAsync();

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = $"{cake.CakeCode} is Price Submitted.",
                Body = $"Hi {cake.User.FirstName}<br/><br/>Your custom cake({cake.CakeCode}) price has been submitted successfully. We can create your cake for Rs.{dto.CakePrice}.00. Thank you for reaching us !.<br/>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(cake.User.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);

            return Ok(new { message = "Price set successfully" });
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCustomCakesByUser(int userId)
        {
            var user = await _context.Users
                .Include(u => u.CustomCakes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var cakes = user.CustomCakes.Select(c => new
            {
                c.Id,
                c.CakeCode,
                c.CakeWeight,
                c.CakePrice,
                c.CakeImageLink1,
                c.CakeImageLink2,
                c.CakeImageLink3,
                c.CakeImageLink4,
                c.CakeImageLink5
            });

            return Ok(cakes);
        }


        public class SetPriceDto
        {
            public double CakePrice { get; set; }
        }


        public class CustomCakeDto
        {
            public int UserId { get; set; }
            public string Description { get; set; }
            public string CakeWeight { get; set; }
            public List<IFormFile> CakeImages { get; set; } // 👈 allow multiple files
        }


        private static string GenerateCakeCode()
        {
            string prefix = "CK";
            Random random = new Random();
            int number = random.Next(1000, 9999);
            return $"{prefix}{number}";
        }

        //public class CustomCakeDto
        //{
        //    public int UserId { get; set; }
        //    public string Description { get; set; }
        //    public string CakeWeight { get; set; }
        //    public string? ImageLink1 { get; set; }
        //    public string? ImageLink2 { get; set; }
        //    public string? ImageLink3 { get; set; }
        //    public string? ImageLink4 { get; set; }
        //    public string? ImageLink5 { get; set; }
        //}
    }
}
