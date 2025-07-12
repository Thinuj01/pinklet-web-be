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
    public class CakeModelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public CakeModelController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        //POST: api
        [HttpPost]
        public async Task<IActionResult> AddCakeWithLayers([FromBody] _3DCakeModel cake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cake == null)
            {
                return BadRequest("Cake data is required.");
            }

            // Generate unique CakeCode
            string code;
            do
            {
                code = GenerateUniqueCakeCode();
            } while (await _context.Cakes3dModel.AnyAsync(c => c.CakeCode == code));

            cake.CakeCode = code;

            // Avoid circular references
            foreach (var layer in cake.CakeLayers)
            {
                layer.Cake = null;
            }

            _context.Cakes3dModel.Add(cake);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cake and layers added successfully", cakeId=cake.Id });
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetCakeWithLayers(int id)
        {
            var cake = await _context.Cakes3dModel
                .Include(c => c.CakeLayers)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cake == null)
            {
                return NotFound("Cake not found.");
            }
            // Remove circular reference if present
            foreach (var layer in cake.CakeLayers)
            {
                layer.Cake = null;
            }
            return Ok(cake);
        }


        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetCake()
        {
            
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User ID claim missing in token.");
            }

            
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user ID format.");
            }

           
            var cakes = await _context.Cakes3dModel
                .Include(c => c.CakeLayers)
                .Where(c => c.UserId == userId)
                .ToListAsync();

           
            foreach (var cake in cakes)
            {
                foreach (var layer in cake.CakeLayers)
                {
                    layer.Cake = null;
                }
            }

            return Ok(cakes);
        }

        [HttpPut("request/{id}")]
        public async Task<IActionResult> MarkCakeAsRequested(int id)
        {
            // Find the cake by ID
            var cake = await _context.Cakes3dModel.FirstOrDefaultAsync(c => c.Id == id);

            if (cake == null)
            {
                return NotFound("Cake not found.");
            }

            // Set IsReqested to true
            cake.IsReqested = true;

            // Save changes to DB
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cake marked as requested successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCake(int id, [FromBody] _3DCakeModel updatedCake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updatedCake == null || updatedCake.CakeLayers == null)
            {
                return BadRequest("Cake and layers data is required.");
            }

            var existingCake = await _context.Cakes3dModel
                .Include(c => c.CakeLayers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCake == null)
            {
                return NotFound("Cake not found.");
            }

            // Update cake properties
            existingCake.Occation = updatedCake.Occation;
            existingCake.BaseShape = updatedCake.BaseShape;
            existingCake.BaseShapeSize = updatedCake.BaseShapeSize;
            existingCake.NoLayers = updatedCake.NoLayers;
            existingCake.LayerShape = updatedCake.LayerShape;
            existingCake.IcingType = updatedCake.IcingType;
            existingCake.Toppers = updatedCake.Toppers; 
            existingCake.IsReqested = updatedCake.IsReqested;

            // Remove old layers
            _context.CakeLayers.RemoveRange(existingCake.CakeLayers);

            // Add new layers
            foreach (var layer in updatedCake.CakeLayers)
            {
                layer.CakeId = id;
                _context.CakeLayers.Add(layer);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cake updated successfully.", cakeId = existingCake.Id });
        }
        private string GenerateUniqueCakeCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();

            string letters = new string(Enumerable.Range(0, 3)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());

            string numbers = random.Next(10000, 99999).ToString("D5");

            return letters + numbers;
        }



    }


}
