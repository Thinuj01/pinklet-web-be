using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace pinklet.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        [NotMapped]
        [JsonIgnore]
        public User Vendor { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public string ItemTags { get; set; }
        public double ItemPrice { get; set; }
        public int ItemRating { get; set; }
        public int ItemDescription { get; set; }
        public string? ItemImageLink1 { get; set; }
        public string? ItemImageLink2 { get; set; }
        public string? ItemImageLink3 { get; set; }
        public string? ItemImageLink4 { get; set; }

        public ICollection<ItemPackage> ItemPackages { get; set; } = new List<ItemPackage>();
    }
}
