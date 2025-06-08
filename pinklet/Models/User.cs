using System.ComponentModel.DataAnnotations;

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
        public DateTime? TokenGeneratedAt { get; set; }
    }
}
