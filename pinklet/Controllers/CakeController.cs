using Microsoft.AspNetCore.Mvc;
using pinklet.data;
using Microsoft.EntityFrameworkCore;
using pinklet.Models;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CakeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public CakeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
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
    }
}
