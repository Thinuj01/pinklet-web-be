using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pinklet.data;
using pinklet.Models;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly CloudinaryService _cloudinaryService;

        public ItemController(ApplicationDbContext context, IConfiguration configuration, CloudinaryService cloudinaryService)
        {
            _context = context;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddItem([FromForm] ItemCreateDto itemDto)
        {
            try
            {
                // Upload image files (up to 5)
                var uploadedUrls = new List<string>();
                foreach (var file in itemDto.ItemImages.Where(f => f != null))
                {
                    var url = await _cloudinaryService.UploadImageAsync(file);
                    uploadedUrls.Add(url);
                }

                // Create item
                var item = new Item
                {
                    ItemName = itemDto.ItemName,
                    ItemCategory = itemDto.ItemCategory,
                    ItemSubCategory = itemDto.ItemSubCategory,
                    ItemDescription = itemDto.ItemDescription,
                    ItemStock = itemDto.ItemStock,
                    ItemTags = itemDto.ItemTags,
                    VendorId = itemDto.VendorId,
                    ItemRating = 0,
                    ItemImageLink1 = uploadedUrls.ElementAtOrDefault(0),
                    ItemImageLink2 = uploadedUrls.ElementAtOrDefault(1),
                    ItemImageLink3 = uploadedUrls.ElementAtOrDefault(2),
                    ItemImageLink4 = uploadedUrls.ElementAtOrDefault(3),
                    ItemImageLink5 = uploadedUrls.ElementAtOrDefault(4)
                };

                item.Vendor = null;

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Item added successfully", item.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "An error occurred while saving the item.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/item
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items
                .Include(i => i.Vendor)
                .Select(i => new ItemWithVendorDTO
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    ItemName = i.ItemName,
                    ItemCategory = i.ItemCategory,
                    ItemSubCategory = i.ItemSubCategory,
                    ItemTags = i.ItemTags,
                    ItemStock = i.ItemStock,
                    ItemPrice = i.ItemPrice,
                    VendorId = i.VendorId,
                    VendorName = i.Vendor.FirstName + " " + i.Vendor.LastName,
                    VendorEmail = i.Vendor.Email,
                    ImageUrl1 = i.ItemImageLink1,
                    ImageUrl2 = i.ItemImageLink2,
                    ImageUrl3 = i.ItemImageLink3,
                    ImageUrl4 = i.ItemImageLink4,
                    ImageUrl5 = i.ItemImageLink5,
                    ItemRate = i.ItemRating,
                })
                .ToListAsync();

            if (items == null || !items.Any())
            {
                return NotFound("No items found.");
            }

            return Ok(items);
        }

        public class ItemWithVendorDTO
        {
            public int Id { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public string ItemSubCategory { get; set; }
            public string ItemTags { get; set; }
            public int ItemStock { get; set; }
            public double ItemPrice { get; set; }
            public int VendorId { get; set; }
            public string VendorName { get; set; }
            public string VendorEmail { get; set; }
            public string? ImageUrl1 { get; set; }
            public string? ImageUrl2 { get; set; }
            public string? ImageUrl3 { get; set; }
            public string? ImageUrl4 { get; set; }
            public string? ImageUrl5 { get; set; }
            public int ItemRate { get; set; }
        }

        public class ItemCreateDto
        {
            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public string ItemSubCategory { get; set; }
            public string ItemDescription { get; set; }
            public string ItemTags { get; set; }
            public int ItemStock { get; set; }
            public int VendorId { get; set; }
            public List<IFormFile?> ItemImages { get; set; } = new();
        }
    }
}
