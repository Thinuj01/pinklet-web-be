using System.ComponentModel.DataAnnotations;

namespace pinklet.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } // optional: Admin/User

        public string Email { get; set; }
    }
}
