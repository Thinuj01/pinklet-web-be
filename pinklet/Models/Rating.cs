using System.ComponentModel.DataAnnotations;

namespace pinklet.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Rate { get; set; }
        public string? Review { get; set; }
    }
}
