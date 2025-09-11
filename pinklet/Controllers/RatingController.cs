using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pinklet.data;
using pinklet.Dto;
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
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token.");

                var user = await _context.Users.FindAsync(int.Parse(userIdClaim));
                if (user == null)
                    return NotFound("User not found.");

                var rating = new pinklet.Models.Rating
                {
                    ItemId = dto.ItemId,
                    UserId = int.Parse(userIdClaim),
                    Rate = dto.Rate,
                    Review = dto.Review
                };

                _context.Rating.Add(rating);
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
