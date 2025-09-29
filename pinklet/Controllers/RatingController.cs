using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pinklet.data;
using pinklet.Dto;
using pinklet.Models;
using System.Security.Claims;

namespace pinklet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration configuration;

        public RatingController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRating([FromBody] RatingDto dto)
        {
            try
            {
                // Validate input
                if (dto == null || dto.ItemId <= 0 || dto.Rate < 1 || dto.Rate > 5)
                    return BadRequest("Invalid item ID or rating. Rating must be between 1 and 5.");

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token.");

                var userId = int.Parse(userIdClaim);
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound("User not found.");

                var item = await _context.Items.FindAsync(dto.ItemId);
                if (item == null)
                    return NotFound("Item not found.");

                // Create new rating
                var rating = new Rating
                {
                    ItemId = dto.ItemId,
                    UserId = userId,
                    Rate = dto.Rate,
                    Review = dto.Review
                };

                _context.Rating.Add(rating);
                await _context.SaveChangesAsync();

                // Update item rating and rating count
                var newRatingNo = (item.RatingNo ?? 0) + 1; // Handle nullable RatingNo
                var newRate = (float)((item.ItemRating * (item.RatingNo ?? 0) + dto.Rate) / (float)newRatingNo);

                // Since ItemRating is int, round or truncate as needed
                item.ItemRating = (int)Math.Round(newRate); // Round to nearest integer
                item.RatingNo = newRatingNo;

                _context.Items.Update(item);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "An error occurred while saving the rating.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet("item/{itemId}")]
        public async Task<IActionResult> GetRatingsForItem(int itemId)
        {
            try
            {
                var ratings = await _context.Rating
             .Where(r => r.ItemId == itemId)
             .Include(r => r.User)
             .Select(r => new
             {
                 r.Id,
                 r.ItemId,
                 r.UserId,
                 r.Rate,
                 r.Review,
                 User = new
                 {
                     r.User.FirstName,
                     r.User.LastName,
                     r.User.ProfileImageLink                 }
             })
             .ToListAsync();
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "An error occurred while retrieving the ratings.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }

        }
    }
}
