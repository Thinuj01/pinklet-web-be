using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pinklet.data;
using pinklet.Dto;
using pinklet.Models;
using System.Net;
using System.Net.Mail;
using System.Numerics;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly CloudinaryService _cloudinaryService;
        private readonly EmailSettings _emailSettings;

        public ItemController(ApplicationDbContext context, IConfiguration configuration, CloudinaryService cloudinaryService, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
            _emailSettings = emailSettings.Value;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddItem([FromForm] ItemCreateDto itemDto)
        {
            try
            {
                // ✅ Check vendor existence
                var vendor = await _context.Vendors
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == itemDto.VendorId);
                if (vendor == null)
                {
                    return BadRequest(new { success = false, message = "Invalid VendorId. Vendor does not exist." });
                }

                // Upload images
                var uploadedUrls = new List<string>();
                foreach (var file in itemDto.ItemImages.Where(f => f != null))
                {
                    var url = await _cloudinaryService.UploadImageAsync(file);
                    uploadedUrls.Add(url);
                }

                // Generate ItemCode
                string itemCode = $"ITEM-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

                var item = new Item
                {
                    ItemCode = itemCode,
                    ItemName = itemDto.ItemName,
                    ItemCategory = itemDto.ItemCategory,
                    ItemSubCategory = itemDto.ItemSubCategory,
                    ItemDescription = itemDto.ItemDescription,
                    ItemStock = itemDto.ItemStock,
                    ItemPrice = itemDto.ItemPrice,
                    ItemTags = itemDto.ItemTags,
                    VendorId = itemDto.VendorId,
                    ItemVariant = itemDto.ItemVariant,
                    ItemRating = 0,
                    ItemImageLink1 = uploadedUrls.ElementAtOrDefault(0),
                    ItemImageLink2 = uploadedUrls.ElementAtOrDefault(1),
                    ItemImageLink3 = uploadedUrls.ElementAtOrDefault(2),
                    ItemImageLink4 = uploadedUrls.ElementAtOrDefault(3),
                    ItemImageLink5 = uploadedUrls.ElementAtOrDefault(4),
                    IsVerified = false,
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = $"{item.ItemName} is now on verification process",
                    Body = $"Hi {vendor.FullName},<br/>Your {item.ItemName} is send into varification.This may be take 24 hours.Stay Tuned",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(vendor.User.Email);

                using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mailMessage);

                return Ok(new { success = true, message = "Item added successfully", item.Id, item.ItemCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "An error occurred while saving the item.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/item
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items
                .Include(i => i.Vendor)
                .ThenInclude(v => v.User)
                .Select(i => new ItemWithVendorDTO
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    ItemName = i.ItemName,
                    ItemCategory = i.ItemCategory,
                    ItemSubCategory = i.ItemSubCategory,
                    ItemTags = i.ItemTags,
                    ItemStock = i.ItemStock,
                    ItemPrice = i.ItemPrice,
                    ItemVariant = i.ItemVariant,
                    VendorId = i.VendorId,
                    ItemDescription = i.ItemDescription,

                    
                    ShopName = i.Vendor.ShopName,
                    ShopCity = i.Vendor.ShopCity,
                    ShopDistrict = i.Vendor.ShopDistrict,
                    IsVerified = i.Vendor.IsVerified ?? false,

                    
                    ImageUrl1 = i.ItemImageLink1,
                    ImageUrl2 = i.ItemImageLink2,
                    ImageUrl3 = i.ItemImageLink3,
                    ImageUrl4 = i.ItemImageLink4,
                    ImageUrl5 = i.ItemImageLink5,

                    ItemRate = i.ItemRating
                })
                .ToListAsync();

            if (items == null || !items.Any())
            {
                return NotFound("No items found.");
            }

            return Ok(items);
        }

        [HttpGet("get/{itemCode}")]
        public async Task<IActionResult> GetItemByItemCode(string itemCode)
        {
            try
            {
                var item = await _context.Items
                    .Include(i => i.Vendor)
                    .ThenInclude(v => v.User)
                    .Where(i => i.ItemCode == itemCode)
                    .Select(i => new ItemWithVendorDTO
                    {
                        Id = i.Id,
                        ItemCode = i.ItemCode,
                        ItemName = i.ItemName,
                        ItemCategory = i.ItemCategory,
                        ItemSubCategory = i.ItemSubCategory,
                        ItemTags = i.ItemTags,
                        ItemStock = i.ItemStock,
                        ItemPrice = i.ItemPrice,
                        ItemVariant = i.ItemVariant,
                        VendorId = i.VendorId,
                        ItemDescription = i.ItemDescription,

                        ShopName = i.Vendor.ShopName,
                        ShopCity = i.Vendor.ShopCity,
                        ShopDistrict = i.Vendor.ShopDistrict,
                        IsVerified = i.Vendor.IsVerified ?? false,

                        ImageUrl1 = i.ItemImageLink1,
                        ImageUrl2 = i.ItemImageLink2,
                        ImageUrl3 = i.ItemImageLink3,
                        ImageUrl4 = i.ItemImageLink4,
                        ImageUrl5 = i.ItemImageLink5,

                        ItemRate = i.ItemRating
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound(new { success = false, message = "Item not found." });
                }

                return Ok(new { success = true, item });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { success = false, error = "Internal Server Error", details = inner });
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateItem(int id, [FromForm] ItemUpdateDto updateDto)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
                if (item == null)
                    return NotFound(new { success = false, message = "Item not found." });
                
                // Update editable fields
                if (!string.IsNullOrWhiteSpace(updateDto.ItemName))
                    item.ItemName = updateDto.ItemName;

                if (!string.IsNullOrWhiteSpace(updateDto.ItemDescription))
                    item.ItemDescription = updateDto.ItemDescription;

                // If variants are sent, update them
                if (!string.IsNullOrWhiteSpace(updateDto.ItemVariants))
                    item.ItemVariant = updateDto.ItemVariants;

                // If no variants, update stock & price
                if (!updateDto.ItemVariantHasValues)
                {
                    if (updateDto.ItemStock.HasValue)
                        item.ItemStock = updateDto.ItemStock;

                    if (updateDto.ItemPrice.HasValue)
                        item.ItemPrice = updateDto.ItemPrice;
                }

                _context.Items.Update(item);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Item updated successfully", item.Id, updateDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "An error occurred while updating the item.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        public class ItemUpdateDto
        {
            public string? ItemName { get; set; }
            public string? ItemDescription { get; set; }
            public int? ItemStock { get; set; }
            public double? ItemPrice { get; set; }

            // JSON string for variants
            public string? ItemVariants { get; set; }

            // Helper to know if variants exist
            public bool ItemVariantHasValues => !string.IsNullOrWhiteSpace(ItemVariants);
        }


        public class UserDetailsItem {
            public int Id { get; set; }
            public VendorDetailsItem Vendor { get; set; }
        }

        public class VendorDetailsItem
        {
            public int Id { get; set; }
            public string ShopName { get; set; }
            public string FullName { get; set; }
        }

        public class ItemWithVendorDTO
        {
            public int Id { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public string ItemSubCategory { get; set; }
            public string ItemTags { get; set; }
            public int? ItemStock { get; set; }
            public double? ItemPrice { get; set; }
            public string? ItemVariant { get; set; }
            public string ItemDescription { get; set; }

            public int VendorId { get; set; }

            // Vendor info (from Vendor table)
            public string ShopName { get; set; }
            public string ShopCity { get; set; }
            public string ShopDistrict { get; set; }
            public bool IsVerified { get; set; }

            // Images
            public string? ImageUrl1 { get; set; }
            public string? ImageUrl2 { get; set; }
            public string? ImageUrl3 { get; set; }
            public string? ImageUrl4 { get; set; }
            public string? ImageUrl5 { get; set; }

            public int ItemRate { get; set; }
        }
        public class ItemWithVendorall
        {
            public int Id { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public string ItemSubCategory { get; set; }
            public string ItemTags { get; set; }
            public int? ItemStock { get; set; }
            public double? ItemPrice { get; set; }
            public string? ItemVariant { get; set; }
            public string ItemDescription { get; set; }

            public int VendorId { get; set; }

            // Vendor info (from Vendor table)
            public string ShopName { get; set; }
            public string ShopCity { get; set; }
            public string ShopDistrict { get; set; }
            public bool IsVerified { get; set; }
            public string FullName { get; set; }

            // Images
            public string? ImageUrl1 { get; set; }
            public string? ImageUrl2 { get; set; }
            public string? ImageUrl3 { get; set; }
            public string? ImageUrl4 { get; set; }
            public string? ImageUrl5 { get; set; }

            public int ItemRate { get; set; }
        }

        // GET: api/item/vendor/{vendorId}
        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetItemsByVendor(int vendorId)
        {
            try
            {
                var items = await _context.Items
                    .Where(i => i.VendorId == vendorId)
                    .Select(i => new ItemWithVendorall
                    {
                        Id = i.Id,
                        ItemCode = i.ItemCode,
                        ItemName = i.ItemName,
                        ItemCategory = i.ItemCategory,
                        ItemSubCategory = i.ItemSubCategory,
                        ItemTags = i.ItemTags,
                        ItemStock = i.ItemStock,
                        ItemPrice = i.ItemPrice,
                        ItemVariant = i.ItemVariant,
                        VendorId = i.VendorId,
                        ItemDescription = i.ItemDescription,

                        ShopName = i.Vendor.ShopName,
                        ShopCity = i.Vendor.ShopCity,
                        ShopDistrict = i.Vendor.ShopDistrict,
                        IsVerified = i.Vendor.IsVerified ?? false,
                        FullName = i.Vendor.FullName,

                        ImageUrl1 = i.ItemImageLink1,
                        ImageUrl2 = i.ItemImageLink2,
                        ImageUrl3 = i.ItemImageLink3,
                        ImageUrl4 = i.ItemImageLink4,
                        ImageUrl5 = i.ItemImageLink5,

                        ItemRate = i.ItemRating
                    })
                    .ToListAsync();

                if (items == null || !items.Any())
                {
                    return NotFound(new { success = false, message = "No items found for this vendor." });
                }

                return Ok(new { success = true, items });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { success = false, error = "Internal Server Error", details = inner });
            }
        }

        // GET: api/item/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingItems()
        {
            var items = await _context.Items
                .Include(i => i.Vendor)
                .ThenInclude(v => v.User)
                .Where(i => i.IsVerified == false)
                .Select(i => new ItemWithVendorall
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    ItemName = i.ItemName,
                    ItemCategory = i.ItemCategory,
                    ItemSubCategory = i.ItemSubCategory,
                    ItemTags = i.ItemTags,
                    ItemStock = i.ItemStock,
                    ItemPrice = i.ItemPrice,
                    ItemVariant = i.ItemVariant,
                    VendorId = i.VendorId,
                    ItemDescription = i.ItemDescription,

                    ShopName = i.Vendor.ShopName,
                    ShopCity = i.Vendor.ShopCity,
                    ShopDistrict = i.Vendor.ShopDistrict,
                    FullName = i.Vendor.FullName,

                    ImageUrl1 = i.ItemImageLink1,
                    ImageUrl2 = i.ItemImageLink2,
                    ImageUrl3 = i.ItemImageLink3,
                    ImageUrl4 = i.ItemImageLink4,
                    ImageUrl5 = i.ItemImageLink5,

                    ItemRate = i.ItemRating
                })
                .ToListAsync();

            return Ok(items);
        }

        // POST: api/item/approve/{id}
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveItem(int id)
        {
            var item = await _context.Items
                .Include(i => i.Vendor)      
                .ThenInclude(v => v.User)     
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound(new { success = false, message = "Item not found." });

            item.IsVerified = true;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            // Email for item approval
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "Item Approved on Pinklet",
                Body = $"Dear {item.Vendor.FullName},<br/>" +
                       $"<p>Your item <b>{item.ItemName}</b> has been approved and is now live on Pinklet!</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(item.Vendor.User.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);

            return Ok(new { success = true, message = "Item approved successfully." });
        }

        // POST: api/item/reject/{id}
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectItem(int id, [FromBody] RejectReasonDto dto)
        {
            var item = await _context.Items
                .Include(i => i.Vendor)
                .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound(new { success = false, message = "Item not found." });

            // Option 1: Remove the item
            _context.Items.Remove(item);

            //// Option 2: Mark as rejected and store reason
            //item.IsVerified = false;
            //_context.Items.Update(item);

            await _context.SaveChangesAsync();

            // Email for item rejection
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "Item Rejected on Pinklet",
                Body = $"Dear {item.Vendor.FullName},<br/>" +
                       $"<p>Unfortunately, your item <b>{item.ItemName}</b> was rejected.</p>" +
                       $"<p><b>Reason:</b> {dto.Reason}</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(item.Vendor.User.Email);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mailMessage);

            return Ok(new { success = true, message = "Item rejected.", reason = dto.Reason });
        }


        public class RejectReasonDto
        {
            public string Reason { get; set; }
        }


        public class ItemCreateDto
        {
            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public string ItemSubCategory { get; set; }
            public string ItemDescription { get; set; }
            public string ItemTags { get; set; }
            public double? ItemPrice { get; set; }
            public int? ItemStock { get; set; }
            public string? ItemVariant { get; set; }
            public int VendorId { get; set; }
            public List<IFormFile?> ItemImages { get; set; } = new();
        }
    }
}
