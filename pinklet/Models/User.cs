using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Availability {  get; set; }

        public string? EmailVerificationToken { get; set; }
        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset? TokenGeneratedAt { get; set; }

        public string? ProfileImageLink { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();

        public ICollection<Package> Packages { get; set; } = new List<Package>();

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
