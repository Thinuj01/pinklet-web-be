using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace pinklet.Models
{
    public class ItemPackage
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }
        
        public int Quantity { get; set; } = 1;
        public int? Variant { get; set; }
        
    }
}
