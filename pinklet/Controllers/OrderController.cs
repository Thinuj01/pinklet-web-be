using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pinklet.data;
using pinklet.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using pinklet.Dto;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;
        private readonly EmailSettings _emailSettings;

        public OrderController(ApplicationDbContext context, IConfiguration configuration, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            this.configuration = configuration;
            _emailSettings = emailSettings.Value;
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : (int?)null;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDto dto)
        {
            try
            {
                if (!ModelState.IsValid || dto == null)
                    return BadRequest(ModelState);

                // Create Order
                var order = new Order
                {
                    OrderId = dto.OrderId,
                    UserId = dto.UserId,
                    CartId = dto.CartId,
                    TotalAmount = dto.TotalAmount,
                    Address = dto.Address,
                    RecipientName = dto.RecipientName,
                    ClientPhoneNumber = dto.ClientPhoneNumber,
                    RecipientPhoneNumber = dto.RecipientPhoneNumber,
                    Progress = dto.Progress,
                    OrderedDate = DateTime.Now.ToUniversalTime(),
                    District = dto.District,
                    PostalCode = dto.PostalCode,
                    DeliveryNote = dto.DeliveryNote,
                    OrderNote = dto.OrderNote,
                    ClientEmailAddress = dto.ClientEmailAddress,
                    ClientFullName = dto.ClientFullName
                };

                _context.Orders.Add(order);

                // ✅ Get Cart and Package
                var cart = await _context.Carts
                    .Include(c => c.Package)
                        .ThenInclude(p => p.ItemPackages)
                            .ThenInclude(ip => ip.Item)
                                .ThenInclude(i => i.Vendor)
                    .FirstOrDefaultAsync(c => c.Id == dto.CartId);

                if (cart == null || cart.Package == null)
                    return BadRequest("Cart or Package not found");

                // ✅ Generate OrderItems
                var orderItems = cart.Package.ItemPackages.Select(ip => new OrderItem
                {
                    Order = order,
                    ItemPackageId = ip.Id, // If you have composite key, handle accordingly
                    VendorId = ip.Item.VendorId,
                    Progress = "Pending"
                }).ToList();

                _context.OrderItems.AddRange(orderItems);

                // ✅ Mark cart as checked out
                cart.IsCheckedOut = true;
                _context.Carts.Update(cart);

                await _context.SaveChangesAsync();

                // ✅ Send Confirmation Email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = $"{cart.Package.PackageName} is Ordered Successfully.",
                    Body = $"Hi {dto.ClientFullName}<br/><br/>Your package has been ordered successfully with payment of Rs.{dto.TotalAmount}.00. " +
                           $"Vendors will update progress individually. You will be notified via email or check in your account.<br/><br/>Thank you for shopping with us!",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(dto.ClientEmailAddress);

                using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mailMessage);

                return Ok(new { message = "Order Added", order.Id });
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? "No Inner exception";
                return StatusCode(500, $"Internal Server Error: {inner}");
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrder()
        {
            try
            {
                var tokenUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ItemPackage)
                            .ThenInclude(ip => ip.Item)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Vendor)
                    .Where(o => o.UserId == int.Parse(tokenUserId))
                    .ToListAsync();

                if (orders == null || orders.Count == 0)
                    return NotFound("No orders found for the specified user.");

                var result = orders.Select(order => new
                {
                    order.Id,
                    order.OrderId,
                    order.TotalAmount,
                    order.Address,
                    order.ClientFullName,
                    order.Progress,
                    order.OrderedDate,
                    OrderItems = order.OrderItems.Select(oi => new
                    {
                        oi.Id,
                        oi.Progress,
                        Vendor = new { oi.Vendor.Id, oi.Vendor.ShopName },
                        Item = new { oi.ItemPackage.Item.ItemName, oi.ItemPackage.Quantity }
                    })
                });

                return Ok(result);
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? "No Inner exception";
                return StatusCode(500, $"Internal Server Error: {inner}");
            }
        }

        [HttpGet("vendor/{vendorId}")]
        [Authorize]
        public async Task<IActionResult> GetPendingOrderItemsByVendor(int vendorId)
        {
            try
            {
                var orderItems = await _context.OrderItems
                    .Include(oi => oi.Order) // include Order
                    .Include(oi => oi.ItemPackage)
                        .ThenInclude(ip => ip.Item) // include Item inside ItemPackage
                    .Where(oi => oi.ItemPackage.Item.VendorId == vendorId && oi.Progress == "Pending") // filter by vendor
                    .ToListAsync();

                if (orderItems == null || orderItems.Count == 0)
                {
                    return NotFound("No order items found for this vendor.");
                }

                var result = orderItems.Select(oi => new
                {
                    OrderItemId = oi.Id,
                    Progress = oi.Progress,

                    // Parent Order Info
                    Order = new
                    {
                        oi.Order.Id,
                        oi.Order.OrderId,
                        oi.Order.OrderedDate,
                        oi.Order.ClientFullName,
                        oi.Order.Address,
                        oi.Order.District,
                        oi.Order.PostalCode
                    },

                    // ItemPackage Info
                    ItemPackage = new
                    {
                        oi.ItemPackage.Id,
                        oi.ItemPackage.Quantity,
                        oi.ItemPackage.Variant,

                        Item = new
                        {
                            oi.ItemPackage.Item.Id,
                            oi.ItemPackage.Item.ItemName,
                            oi.ItemPackage.Item.ItemPrice,
                            oi.ItemPackage.Item.ItemStock,
                            oi.ItemPackage.Item.ItemCategory,
                            oi.ItemPackage.Item.ItemSubCategory,
                            oi.ItemPackage.Item.ItemDescription,
                            oi.ItemPackage.Item.ItemImageLink1,
                            oi.ItemPackage.Item.ItemImageLink2,
                            oi.ItemPackage.Item.ItemImageLink3,
                            oi.ItemPackage.Item.ItemImageLink4,
                            oi.ItemPackage.Item.ItemImageLink5,
                            oi.ItemPackage.Item.ItemVariant
                        }
                    }
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("ship/{orderItemId}")]
        public async Task<IActionResult> MarkAsShipped(int orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order) // optional if you want to update Order too
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

            if (orderItem == null)
                return NotFound(new { message = "Order item not found." });

            // Example: update status
            orderItem.Progress = "Shipped";
            //orderItem.ShippedDate = DateTime.UtcNow; // if you have a field

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order item marked as shipped.", orderItemId = orderItemId });
        }


        [HttpGet("vendor/{vendorId}/shipped")]
        public async Task<IActionResult> GetVendorShippedOrders(int vendorId)
        {
            var shippedItems = await _context.OrderItems
                .Include(oi => oi.ItemPackage)
                    .ThenInclude(ip => ip.Item)
                .Include(oi => oi.Order)
                .Where(oi => oi.ItemPackage.Item.VendorId == vendorId && oi.Progress == "Shipped")
                .ToListAsync();

            if (shippedItems == null || shippedItems.Count == 0)
            {
                return NotFound("No order items found for this vendor.");
            }

            var result = shippedItems.Select(oi => new
            {
                OrderItemId = oi.Id,
                Progress = oi.Progress,

                // Parent Order Info
                Order = new
                {
                    oi.Order.Id,
                    oi.Order.OrderId,
                    oi.Order.OrderedDate,
                    oi.Order.ClientFullName,
                    oi.Order.Address,
                    oi.Order.District,
                    oi.Order.PostalCode
                },

                // ItemPackage Info
                ItemPackage = new
                {
                    oi.ItemPackage.Id,
                    oi.ItemPackage.Quantity,
                    oi.ItemPackage.Variant,

                    Item = new
                    {
                        oi.ItemPackage.Item.Id,
                        oi.ItemPackage.Item.ItemName,
                        oi.ItemPackage.Item.ItemPrice,
                        oi.ItemPackage.Item.ItemStock,
                        oi.ItemPackage.Item.ItemCategory,
                        oi.ItemPackage.Item.ItemSubCategory,
                        oi.ItemPackage.Item.ItemDescription,
                        oi.ItemPackage.Item.ItemImageLink1,
                        oi.ItemPackage.Item.ItemImageLink2,
                        oi.ItemPackage.Item.ItemImageLink3,
                        oi.ItemPackage.Item.ItemImageLink4,
                        oi.ItemPackage.Item.ItemImageLink5,
                        oi.ItemPackage.Item.ItemVariant
                    }
                }
            }).ToList();

            return Ok(result);
        }

        [HttpGet("vendor/{vendorId}/completed")]
        public async Task<IActionResult> GetVendorCompletedOrders(int vendorId)
        {
            var completedOrders = await _context.OrderItems
                .Include(oi => oi.ItemPackage)
                    .ThenInclude(ip => ip.Item)
                .Include(oi => oi.Order)
                .Where(oi => oi.ItemPackage.Item.VendorId == vendorId && oi.Progress == "Collected")
                .ToListAsync();

            if (!completedOrders.Any())
            {
                return NotFound("No completed orders found for this vendor.");
            }


            var result = completedOrders.Select(oi => new
            {
                OrderItemId = oi.Id,
                Progress = oi.Progress,

                // Parent Order Info
                Order = new
                {
                    oi.Order.Id,
                    oi.Order.OrderId,
                    oi.Order.OrderedDate,
                    oi.Order.ClientFullName,
                    oi.Order.Address,
                    oi.Order.District,
                    oi.Order.PostalCode
                },

                // ItemPackage Info
                ItemPackage = new
                {
                    oi.ItemPackage.Id,
                    oi.ItemPackage.Quantity,
                    oi.ItemPackage.Variant,

                    Item = new
                    {
                        oi.ItemPackage.Item.Id,
                        oi.ItemPackage.Item.ItemName,
                        oi.ItemPackage.Item.ItemPrice,
                        oi.ItemPackage.Item.ItemStock,
                        oi.ItemPackage.Item.ItemCategory,
                        oi.ItemPackage.Item.ItemSubCategory,
                        oi.ItemPackage.Item.ItemDescription,
                        oi.ItemPackage.Item.ItemImageLink1,
                        oi.ItemPackage.Item.ItemImageLink2,
                        oi.ItemPackage.Item.ItemImageLink3,
                        oi.ItemPackage.Item.ItemImageLink4,
                        oi.ItemPackage.Item.ItemImageLink5,
                        oi.ItemPackage.Item.ItemVariant
                    }
                }
            }).ToList();

            return Ok(result);
        }


    }
}
