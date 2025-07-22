using Microsoft.AspNetCore.Mvc;
using pinklet.data;
using pinklet.Models;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly CloudinaryService _cloudinaryService;

        public VendorController(ApplicationDbContext context, IConfiguration configuration, CloudinaryService cloudinaryService)
        {
            _context = context;
            this.configuration = configuration;
            _cloudinaryService = cloudinaryService;
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
