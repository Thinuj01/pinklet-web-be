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
        public async Task<IActionResult> SavePackage([FromBody] Package package)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (package == null)
            {
                return BadRequest("Package data is required.");
            }
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Package added successfully", package.Id });
        }

        [HttpGet]
        public async Task<IActionResult>GetPackageById(int id)
        {
            var package = await _context.Packages
                .Include(p => p.ItemPackages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (package == null)
            {
                return NotFound("Package not found.");
            }
            // Remove circular reference if present
            foreach (var item in package.ItemPackages)
            {
                item.Package = null;
            }
            return Ok(package);
        }

    }
}
