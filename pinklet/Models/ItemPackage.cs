using System.ComponentModel.DataAnnotations.Schema;
namespace pinklet.Models
{
    public class ItemPackage
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }


    }
}
