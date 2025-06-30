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
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item added successfully", item.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items.ToListAsync();
            if (items == null || !items.Any())
            {
                return NotFound("No items found.");
            }
            return Ok(items);
        }
    }
}
