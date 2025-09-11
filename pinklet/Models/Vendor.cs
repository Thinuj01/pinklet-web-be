using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string ShopName { get; set; }
        public string ShopDistrict { get; set; }
        public string ShopCity { get; set; }
        public string ShopDescription { get; set; }
        public string? ShopProfileImageLink { get; set; }
        public string? ShopCoverImageLink { get; set; }
        public string FullName { get; set; }
        public string IDNumber { get; set; }
        public string IDImageLink1 { get; set; }
        public string IDImageLink2 { get; set; }
        public Boolean? IsVerified { get; set; } = false;
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
