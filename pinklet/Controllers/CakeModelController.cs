using Microsoft.AspNetCore.Mvc;
using pinklet.data;
using Microsoft.EntityFrameworkCore;
using pinklet.Models;

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

            
            if (Request.ContentType?.Contains("application/json") == true &&
                HttpContext.Request.Body.CanSeek)
            {
                HttpContext.Request.Body.Position = 0;
                using var reader = new StreamReader(HttpContext.Request.Body);
                var bodyText = await reader.ReadToEndAsync();
                var json = System.Text.Json.JsonDocument.Parse(bodyText);
                if (json.RootElement.TryGetProperty("Toppers", out var toppersElement) &&
                    toppersElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    cake.Toppers = toppersElement.ToString();
                }
            }

            foreach (var layer in cake.CakeLayers)
            {
                layer.Cake = null;
            }

            _context.Cakes3dModel.Add(cake);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cake and layers added successfully", cake.Id });
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
    }

    
}
