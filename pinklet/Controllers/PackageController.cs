using Microsoft.AspNetCore.Mvc;
using pinklet.data;
using Microsoft.EntityFrameworkCore;
using pinklet.Models;

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
                    CakeId = dto.CakeId ?? 0,  // EF requires a value; 0 will be ignored in DB if unused
                    ThreeDCakeId = dto.ThreeDCakeId ?? 0,
                    ItemPackages = dto.ItemIds.Select(id => new ItemPackage { ItemId = id }).ToList()
                };

                _context.Packages.Add(package);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Package added", package.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                return StatusCode(500, "Internal server error occurred.");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var package = await _context.Packages
                .Include(p => p.ItemPackages)
                    .ThenInclude(ip => ip.Item)
                .Include(p => p.Cake)
                .Include(p => p.ThreeDCake)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                return NotFound("Package not found.");
            }

            var dto = new PackageDetailsDTO
            {
                Id = package.Id,
                PackageCode = package.PackageCode,
                UserId = package.UserId,
                Cake = package.CakeId != 0 ? package.Cake : null,
                ThreeDCake = package.ThreeDCakeId != 0 ? package.ThreeDCake : null,
                Items = package.ItemPackages.Select(ip => new ItemDTO
                {
                    Id = ip.Item.Id,
                    ItemCode = ip.Item.ItemCode,
                    ItemName = ip.Item.ItemName,
                    ItemPrice = ip.Item.ItemPrice,
                    ItemCategory = ip.Item.ItemCategory
                }).ToList()
            };

            return Ok(dto);
        }



        public class PackageDTO
        {
            public string PackageCode { get; set; }
            public int UserId { get; set; }
            public int? CakeId { get; set; }  // nullable
            public int? ThreeDCakeId { get; set; }  // nullable
            public List<int> ItemIds { get; set; }
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
        }

    }
}
