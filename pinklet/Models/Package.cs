using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class Package
    {
        [Key]
        public int Id { get; set; }
        public string PackageCode { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<ItemPackage> ItemPackages { get; set; } = new List<ItemPackage>();

        public int? CakeId { get; set; }
        [ForeignKey(nameof(CakeId))]
        public Cake Cake { get; set; }

        public int? ThreeDCakeId { get; set; }
        [ForeignKey(nameof(ThreeDCakeId))]
        [InverseProperty(nameof(_3DCakeModel.Packages))]
        public _3DCakeModel ThreeDCake { get; set; }

        public int? CustomCakeId { get; set; }
        [ForeignKey(nameof(CustomCakeId))]
        public CustomCake CustomCake { get; set; }

        public ICollection<Cart> Carts { get; set; }
        public string PackageName { get; set; }
        public double TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public int TotalCategories { get; set; }
        public string District { get; set; }

    }
}
