using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pinklet.data;
using pinklet.Models;
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
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (carts == null || carts.Count == 0)
            {
                return NotFound("No carts found for the specified user.");
            }

            var result = carts.Select(cart => new CartDetailsDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Package = cart.Package == null ? null : new PackageDto
                {
                    Id = cart.Package.Id,
                    PackageName = cart.Package.PackageName,
                    TotalAmount = cart.Package.TotalAmount,
                    TotalItems = cart.Package.TotalItems,
                    TotalCategories = cart.Package.TotalCategories,
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
                    }
                }
            }).ToList();

            return Ok(result);
        }




        public class CartDto
        {
            public int UserId { get; set; }
            public int PackageId { get; set; }
        }
        public class CartDetailsDto
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public PackageDto Package { get; set; }
        }

        public class PackageDto
        {
            public int Id { get; set; }
            public string PackageName { get; set; }
            public List<ItemPackageDto> ItemPackages { get; set; }
            public double TotalAmount { get; set; }
            public int TotalItems { get; set; }
            public int TotalCategories { get; set; }
            public CakeDto? Cake { get; set; }

        }

        public class CakeDto
        {
            public int Id { get; set; }
            public string CakeCode { get; set; }
            public string CakeName { get; set; }
            public string CakeCategory { get; set; }
            public string CakeTags { get; set; }
            public double CakePrice { get; set; }
            public int CakeRating { get; set; }
            public string CakeDescription { get; set; }
            public string? CakeImageLink1 { get; set; }
            public string? CakeImageLink2 { get; set; }
            public string? CakeImageLink3 { get; set; }
            public string? CakeImageLink4 { get; set; }
        } 

        public class ItemPackageDto
        {
            public ItemDto Item { get; set; }
            public int Quantity { get; set; }
            public int? Variant { get; set; }
        }

        public class ItemDto
        {
            public int Id { get; set; }
            public string ItemName { get; set; }
            public string? ItemVariant { get; set; }
            public string ItemCategory { get; set; }
            public string ItemSubCategory { get; set; }
            public int? ItemStock { get; set; }
            public double? ItemPrice { get; set; }
            public int ItemRating { get; set; }
            public string ItemDescription { get; set; }
            public string? ItemImageLink1 { get; set; }
            public string? ItemImageLink2 { get; set; }
            public string? ItemImageLink3 { get; set; }
            public string? ItemImageLink4 { get; set; }
            public string? ItemImageLink5 { get; set; }

        }

    }
}
