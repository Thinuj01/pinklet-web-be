using pinklet.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int ItemPackageId { get; set; }
        [ForeignKey("ItemPackageId")]
        public ItemPackage ItemPackage { get; set; }

        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }

        // Track vendor-specific progress
        public string Progress { get; set; } = "Pending";
    }
}
