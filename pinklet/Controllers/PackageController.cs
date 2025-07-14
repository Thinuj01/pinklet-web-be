using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pinklet.data;
using pinklet.Models;
using System.Security.Claims;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public PackageController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        // POST: api/package
        [HttpPost]
        public async Task<IActionResult> AddPackage([FromBody] PackageDTO dto)
        {
            try
            {
                if (!ModelState.IsValid || dto == null)
                    return BadRequest("Invalid package data.");

                // Ensure only one cake type is selected
                bool hasCake = dto.CakeId.HasValue;
                bool has3DCake = dto.ThreeDCakeId.HasValue;

                if (hasCake == has3DCake)
                {
                    return BadRequest("You must provide either CakeId or ThreeDCakeId, but not both.");
                }

                var package = new Package
                {
                    PackageCode = dto.PackageCode,
                    UserId = dto.UserId,
                    CakeId = dto.CakeId ?? null,
                    ThreeDCakeId = dto.ThreeDCakeId ?? null,
                    ItemPackages = dto.Items.Select(i => new ItemPackage
                    {
                        ItemId = i.ItemId,
                        Quantity = i.Quantity
                    }).ToList()
                };
                _context.Packages.Add(package);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Package added", package.Id });
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? "No inner exception";
                Console.WriteLine($"❌ DB Update Error: {inner}");
                return StatusCode(500, $"Internal server error: {inner}");
            }

        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPackageByUserId()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found.");
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest("Invalid user ID claim.");
                }

                var package = await _context.Packages
                    .Include(p => p.ItemPackages)
                        .ThenInclude(ip => ip.Item)
                    .Include(p => p.Cake)
                    .Include(p => p.ThreeDCake)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (package == null)
                {
                    return NotFound("Package not found.");
                }

                var dto = new PackageDetailsDTO
                {
                    Id = package.Id,
                    PackageCode = package.PackageCode,
                    UserId = package.UserId,
                    Cake = package.CakeId.HasValue ? package.Cake : null,
                    ThreeDCake = package.ThreeDCakeId.HasValue ? package.ThreeDCake : null,
                    Items = package.ItemPackages
                        .Where(ip => ip.Item != null)
                        .Select(ip => new ItemDTO
                        {
                            Id = ip.Item.Id,
                            ItemCode = ip.Item.ItemCode,
                            ItemName = ip.Item.ItemName,
                            ItemPrice = ip.Item.ItemPrice,
                            ItemCategory = ip.Item.ItemCategory,
                            Quantity = ip.Quantity
                        }).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Exception in GET: {ex.Message}");
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

    }
    public class PackageDTO
    {
        public string PackageCode { get; set; }
        public int UserId { get; set; }
        public int? CakeId { get; set; }
        public int? ThreeDCakeId { get; set; }

        public List<ItemWithQuantityDTO> Items { get; set; }  // Change from List<int> to List<ItemWithQuantityDTO>
    }
    public class ItemWithQuantityDTO
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class PackageDetailsDTO
    {
        public int Id { get; set; }
        public string PackageCode { get; set; }
        public int UserId { get; set; }

        public List<ItemDTO> Items { get; set; }

        public Cake? Cake { get; set; }
        public _3DCakeModel? ThreeDCake { get; set; }
    }

    public class ItemDTO
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }
        public string ItemCategory { get; set; }
        public int Quantity { get; set; }
    }
}
