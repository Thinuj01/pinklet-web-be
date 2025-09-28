using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pinklet.data;
using pinklet.Models;
using pinklet.Dto;
using static pinklet.Controllers.PackageController;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> AddToCart([FromBody] CartDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (dto == null)
            {
                return BadRequest("Cart data is required.");
            }
            var cart = new Cart
            {
                UserId = dto.UserId,
                PackageId = dto.PackageId
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item added to cart successfully", cart.Id });
        }

        //[HttpGet]
        //public async Task<IActionResult>GetCartByUserId([FromQuery]int userId)
        //{
        //    if (userId==null)
        //    {
        //        return BadRequest("User ID is required.");
        //    }
        //    var cart = await _context.Carts
        //        .Include(c => c.Package)
        //        .FirstOrDefaultAsync(c => c.UserId == userId);
        //    if (cart == null)
        //    {
        //        return NotFound("Cart not found for the specified user.");
        //    }

        //    //foreach (var item in cart.Package)
        //    //    {
        //    //        item.Cart = null;
        //    //        item.Item.CartItems = null; // Prevent circular reference in Item
        //    //    }
        //    return Ok(cart);
        //}

        [HttpGet]
        public async Task<IActionResult> GetCartByUserId([FromQuery] int userId)
        {
            var carts = await _context.Carts
                .Include(c => c.Package)
                    .ThenInclude(p => p.ItemPackages)
                        .ThenInclude(ip => ip.Item)
                .Include(c => c.Package)
                    .ThenInclude(p => p.Cake)
                .Include(c => c.Package)
                    .ThenInclude(p => p.ThreeDCake)
                .Include(c => c.Package)
                    .ThenInclude(p => p.CustomCake)
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .ToListAsync();

            if (carts == null || carts.Count == 0)
            {
                return NotFound("No carts found for the specified user.");
            }

            var result = carts.Select(cart => new CartDetailsDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                IsCheckedOut = cart.IsCheckedOut,
                Package = cart.Package == null ? null : new PackageDto
                {
                    Id = cart.Package.Id,
                    PackageName = cart.Package.PackageName,
                    TotalAmount = cart.Package.TotalAmount,
                    TotalItems = cart.Package.TotalItems,
                    TotalCategories = cart.Package.TotalCategories,
                    District = cart.Package.District,
                    ItemPackages = cart.Package.ItemPackages?.Select(ip => new ItemPackageDto
                    {
                        Quantity = ip.Quantity,
                        Variant = ip.Variant,
                        Item = new ItemDto
                        {
                            Id = ip.Item.Id,
                            ItemName = ip.Item.ItemName,
                            ItemVariant = ip.Item.ItemVariant,
                            ItemCategory = ip.Item.ItemCategory,
                            ItemSubCategory = ip.Item.ItemSubCategory,
                            ItemStock = ip.Item.ItemStock,
                            ItemPrice = ip.Item.ItemPrice,
                            ItemRating = ip.Item.ItemRating,
                            ItemDescription = ip.Item.ItemDescription,
                            ItemImageLink1 = ip.Item.ItemImageLink1,
                            ItemImageLink2 = ip.Item.ItemImageLink2,
                            ItemImageLink3 = ip.Item.ItemImageLink3,
                            ItemImageLink4 = ip.Item.ItemImageLink4,
                            ItemImageLink5 = ip.Item.ItemImageLink5
                        },

                    }).ToList(),
                    Cake = cart.Package.Cake == null ? null : new CakeDto
                    {
                        Id = cart.Package.Cake.Id,
                        CakeCode = cart.Package.Cake.CakeCode,
                        CakeName = cart.Package.Cake.CakeName,
                        CakeCategory = cart.Package.Cake.CakeCategory,
                        CakeTags = cart.Package.Cake.CakeTags,
                        CakePrice = cart.Package.Cake.CakePrice,
                        CakeRating = cart.Package.Cake.CakeRating,
                        CakeDescription = cart.Package.Cake.CakeDescription,
                        CakeImageLink1 = cart.Package.Cake.CakeImageLink1,
                        CakeImageLink2 = cart.Package.Cake.CakeImageLink2,
                        CakeImageLink3 = cart.Package.Cake.CakeImageLink3,
                        CakeImageLink4 = cart.Package.Cake.CakeImageLink4,
                    },
                    ThreeDCake = cart.Package.ThreeDCake == null ? null : new ThreeDCakeDto
                    {
                        Id = cart.Package.ThreeDCake.Id,
                        CakeCode = cart.Package.ThreeDCake.CakeCode,
                        RequestedPrice = cart.Package.ThreeDCake.RequestedPrice
                    },
                    CustomCake = cart.Package.CustomCake == null ? null : new CustomCakeDto
                    {
                        Id = cart.Package.CustomCake.Id,
                        CakeCode = cart.Package.CustomCake.CakeCode,
                        CakeWeight = cart.Package.CustomCake.CakeWeight,
                        CakePrice = cart.Package.CustomCake.CakePrice,
                        CakeImageLink1 = cart.Package.CustomCake.CakeImageLink1,
                        CakeImageLink2 = cart.Package.CustomCake.CakeImageLink2,
                        CakeImageLink3 = cart.Package.CustomCake.CakeImageLink3,
                        CakeImageLink4 = cart.Package.CustomCake.CakeImageLink4,
                        CakeImageLink5 = cart.Package.CustomCake.CakeImageLink5
                    },

                }
            }).ToList();

            return Ok(result);
        }

    }
}
