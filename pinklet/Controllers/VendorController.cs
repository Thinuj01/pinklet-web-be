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
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly CloudinaryService _cloudinaryService;
        private readonly EmailSettings _emailSettings;

        public VendorController(ApplicationDbContext context, IConfiguration configuration, CloudinaryService cloudinaryService, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            this.configuration = configuration;
            _cloudinaryService = cloudinaryService;
            _emailSettings = emailSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterVendor([FromForm] VendorCreateDto vendorDto)
        {
            try
            {
                // Upload images if provided
                string profileImageUrl = vendorDto.ShopProfileImage != null
                    ? await _cloudinaryService.UploadImageAsync(vendorDto.ShopProfileImage)
                    : null;

                string coverImageUrl = vendorDto.ShopCoverImage != null
                    ? await _cloudinaryService.UploadImageAsync(vendorDto.ShopCoverImage)
                    : null;

                string idImageUrl1 = vendorDto.IDImage1 != null
                    ? await _cloudinaryService.UploadImageAsync(vendorDto.IDImage1)
                    : null;

                string idImageUrl2 = vendorDto.IDImage2 != null
                    ? await _cloudinaryService.UploadImageAsync(vendorDto.IDImage2)
                    : null;

                // Create Vendor entity
                var vendor = new Vendor
                {
                    UserId = vendorDto.UserId,
                    ShopName = vendorDto.ShopName,
                    ShopDistrict = vendorDto.ShopDistrict,
                    ShopCity = vendorDto.ShopCity,
                    ShopDescription = vendorDto.ShopDescription,
                    FullName = vendorDto.FullName,
                    IDNumber = vendorDto.IDNumber,
                    ShopProfileImageLink = profileImageUrl,
                    ShopCoverImageLink = coverImageUrl,
                    IDImageLink1 = idImageUrl1,
                    IDImageLink2 = idImageUrl2,
                    IsVerified = false
                };

                // Save to database
                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Vendor registered successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "An error occurred while saving the vendor.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetVendorByUserId(int userId)
        {
            var vendor = await _context.Vendors
                .Where(v => v.UserId == userId)
                .Select(v => new
                {
                    v.Id,
                    v.UserId,
                    v.ShopName,
                    v.ShopDistrict,
                    v.ShopCity,
                    v.ShopDescription,
                    v.FullName,
                    v.IDNumber,
                    v.ShopProfileImageLink,
                    v.ShopCoverImageLink,
                    v.IDImageLink1,
                    v.IDImageLink2,
                    v.IsVerified
                })
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                return NotFound(new { success = false, message = "Vendor not found for the given userId." });
            }

            return Ok(new { success = true, vendor });
        }

        [HttpGet("by-vendor/{vendorId}")]
        public async Task<IActionResult> GetVendorBId(int vendorId)
        {
            var vendor = await _context.Vendors
                .Where(v => v.Id == vendorId)
                .Select(v => new
                {
                    v.Id,
                    v.UserId,
                    v.ShopName,
                    v.ShopDistrict,
                    v.ShopCity,
                    v.ShopDescription,
                    v.FullName,
                    v.IDNumber,
                    v.ShopProfileImageLink,
                    v.ShopCoverImageLink,
                    v.IDImageLink1,
                    v.IDImageLink2,
                    v.IsVerified
                })
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                return NotFound(new { success = false, message = "Vendor not found for the given userId." });
            }

            return Ok(new { success = true, vendor });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingVendors()
        {
            var vendors = await _context.Vendors
                .Where(v => v.IsVerified == false)
                .Select(v => new
                {
                    v.Id,
                    v.UserId,
                    v.ShopName,
                    v.ShopDistrict,
                    v.ShopCity,
                    v.ShopDescription,
                    v.FullName,
                    v.IDNumber,
                    v.ShopProfileImageLink,
                    v.ShopCoverImageLink,
                    v.IDImageLink1,
                    v.IDImageLink2,
                    v.IsVerified
                })
                .ToListAsync();

            return Ok(new { success = true, vendors });
        }

        [HttpPost("approve/{vendorId}")]
        public async Task<IActionResult> ApproveVendor(int vendorId)
        {
            var vendor = await _context.Vendors
                .Include(v => v.User) // assuming Vendor has navigation property User
                .FirstOrDefaultAsync(v => v.Id == vendorId);

            if (vendor == null) return NotFound(new { success = false, message = "Vendor not found" });

            vendor.IsVerified = true;
            vendor.User.Role = "Vendor";
            await _context.SaveChangesAsync();

            // send approval email
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "Vendor Account Approved",
                Body = $"Dear {vendor.FullName},<br/> <p>Your vendor account <b>{vendor.ShopName}</b> has been approved. You can now start selling on Pinklet.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(vendor.User.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);

            return Ok(new { success = true, message = "Vendor approved successfully and email sent." });
        }

        [HttpPost("reject/{vendorId}")]
        public async Task<IActionResult> RejectVendor(int vendorId, [FromBody] RejectDto rejectDto)
        {
            var vendor = await _context.Vendors
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == vendorId);

            if (vendor == null) return NotFound(new { success = false, message = "Vendor not found" });

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            // send rejection email
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "Vendor Account Rejected",
                Body = $"Dear {vendor.FullName},<br/> <p>We regret to inform you that your vendor request for <b>{vendor.ShopName}</b> was rejected.</p><p><b>Reason:</b> {rejectDto.Reason}</p><b/><p>Register as vendor again to get approval</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(vendor.User.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);

            return Ok(new { success = true, message = "Vendor rejected, removed, and email sent." });
        }

        public class RejectDto
        {
            public string Reason { get; set; }
        }



        public class VendorCreateDto
        {
            public int UserId { get; set; }
            public string ShopName { get; set; }
            public string ShopDistrict { get; set; }
            public string ShopCity { get; set; }
            public string ShopDescription { get; set; }
            public string FullName { get; set; }
            public string IDNumber { get; set; }

            public IFormFile? ShopProfileImage { get; set; }
            public IFormFile? ShopCoverImage { get; set; }
            public IFormFile? IDImage1 { get; set; }
            public IFormFile? IDImage2 { get; set; }
        }



    }
}
