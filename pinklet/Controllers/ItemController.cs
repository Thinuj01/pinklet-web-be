using Microsoft.AspNetCore.Mvc;
using pinklet.data;
using Microsoft.EntityFrameworkCore;
using pinklet.Models;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public ItemController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        // POST: api/item
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (item == null)
            {
                return BadRequest("Item data is required.");
            }

            // Ensure Vendor is not accidentally populated
            item.Vendor = null;

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item added successfully", item.Id });
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

    }
}
