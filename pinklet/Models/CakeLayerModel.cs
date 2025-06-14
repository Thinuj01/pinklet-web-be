using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class CakeLayerModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CakeId { get; set; }
        [ForeignKey("CakeId")]
        public _3DCakeModel? Cake { get; set; }
        [Required]
        public int LayerNo { get; set; }
        [Required]
        public string LayerFlavor { get; set; }
        [Required]
        public string LayerHeight { get; set; }
        [Required]
        public string LayerColorizeType { get; set; }
        public string LayerSoidColor { get; set; }
        public string LayerGradientColor1 { get; set; }
        public string LayerGradientColor2 { get; set; }
        public string LayerGradientDirection { get; set; }
        public string LayerPatternType { get; set; }
        public string LayerPatternColor { get; set; }
        public string LayerPatternBGColor { get; set; }
    }
}
