using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pinklet.data;
using pinklet.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly ILogger<PackageController> _logger;

        public PackageController(ApplicationDbContext context, IConfiguration configuration, ILogger<PackageController> logger)
        {
            _context = context;
            this.configuration = configuration;
            _logger = logger;
        }

        // Helper to get current user ID
        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : (int?)null;
        }

        // POST: api/package
        [HttpPost]
        public async Task<IActionResult> AddPackage([FromBody] PackageDTO dto)
        {
            try
            {
                if (!ModelState.IsValid || dto == null)
                    return BadRequest(ModelState);

                bool hasCake = dto.CakeId.HasValue;
                bool has3DCake = dto.ThreeDCakeId.HasValue;

                if (hasCake == has3DCake)
                    return BadRequest("You must provide either CakeId or ThreeDCakeId, but not both.");

                var package = new Package
                {
                    PackageCode = dto.PackageCode,
                    UserId = dto.UserId,
                    CakeId = dto.CakeId,
                    ThreeDCakeId = dto.ThreeDCakeId,
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
                _logger.LogError(ex, "Database update error: {Error}", inner);
                return StatusCode(500, $"Internal server error: {inner}");
            }
        }

        // GET: api/package
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPackagesByUserId()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized("Invalid or missing user ID claim.");

                var packages = await _context.Packages
                    .Where(p => p.UserId == userId)
                    .Include(p => p.ItemPackages)
                        .ThenInclude(ip => ip.Item)
                    .Include(p => p.Cake)
                    .Include(p => p.ThreeDCake)
                    .AsNoTracking()
                    .ToListAsync();

                if (packages.Count == 0)
                    return NotFound("No packages found for this user.");

                var packageDtos = packages.Select(package => new PackageDetailsDTO
                {
                    Id = package.Id,
                    PackageCode = package.PackageCode,
                    UserId = package.UserId,
                    Cake = package.CakeId.HasValue ? new Cake
                    {
                        Id = package.Cake.Id,
                        CakeName = package.Cake.CakeName,
                        CakePrice = package.Cake.CakePrice,
                        CakeDescription = package.Cake.CakeDescription,
                        CakeImageLink1 = package.Cake.CakeImageLink1
                    } : null,
                    ThreeDCake = package.ThreeDCakeId.HasValue ? new _3DCakeModel
                    {
                        Id = package.ThreeDCake.Id,
                        CakeCode = package.ThreeDCake.CakeCode,
                        BaseShape = package.ThreeDCake.BaseShape,
                        IcingType = package.ThreeDCake.IcingType
                    } : null,
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
                }).ToList();

                return Ok(packageDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packages by user ID");
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }



        public class PackageDTO
        {
            [Required]
            public string PackageCode { get; set; }

            [Required]
            public int UserId { get; set; }

            public int? CakeId { get; set; }
            public int? ThreeDCakeId { get; set; }

            [Required]
            [MinLength(1, ErrorMessage = "At least one item is required.")]
            public List<ItemWithQuantityDTO> Items { get; set; }
        }

        public class ItemWithQuantityDTO
        {
            [Required]
            public int ItemId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
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
}
