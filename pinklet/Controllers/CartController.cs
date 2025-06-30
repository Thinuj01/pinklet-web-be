using Microsoft.AspNetCore.Mvc;
using pinklet.data;
using Microsoft.EntityFrameworkCore;
using pinklet.Models;

namespace pinklet.Controllers
{
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public CartController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }


        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (cart == null)
            {
                return BadRequest("Cart data is required.");
            }
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item added to cart successfully", cart.Id });
        }

        [HttpGet]
        public async Task<IActionResult>GetCartByUserId(int userId)
        {
            if (userId==null)
            {
                return BadRequest("User ID is required.");
            }
            var cart = await _context.Carts
                .Include(c => c.Package)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return NotFound("Cart not found for the specified user.");
            }
            // Remove circular reference if present
            //foreach (var item in cart.Package)
            //{
            //    item.Cart = null;
            //    item.Item.CartItems = null; // Prevent circular reference in Item
            //}
            return Ok(cart);
        }

    }
}
