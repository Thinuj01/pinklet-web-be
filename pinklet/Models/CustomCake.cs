using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class CustomCake
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string CakeCode { get; set; }
        public string Description { get; set; }
        public string CakeWeight { get; set; }
        public double? CakePrice { get; set; }
        public string? CakeImageLink1 { get; set; }
        public string? CakeImageLink2 { get; set; }
        public string? CakeImageLink3 { get; set; }
        public string? CakeImageLink4 { get; set; }
        public string? CakeImageLink5 { get; set; }
    }
}
