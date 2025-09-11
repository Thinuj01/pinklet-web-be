using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace pinklet.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        public Vendor? Vendor { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public string ItemSubCategory { get; set; }
        public string ItemTags { get; set; }
        public int? ItemStock { get; set; }
        public double? ItemPrice { get; set; }
        public string? ItemVariant { get; set; }
        public int ItemRating { get; set; }
        public string ItemDescription { get; set; }
        public string? ItemImageLink1 { get; set; }
        public string? ItemImageLink2 { get; set; }
        public string? ItemImageLink3 { get; set; }
        public string? ItemImageLink4 { get; set; }
        public string? ItemImageLink5 { get; set; }
        public Boolean? IsVerified { get; set; } = false;

        public ICollection<ItemPackage> ItemPackages { get; set; } = new List<ItemPackage>();
    }
}
