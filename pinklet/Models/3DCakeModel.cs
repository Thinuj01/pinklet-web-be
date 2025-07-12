using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pinklet.Models
{
    public class _3DCakeModel
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Occation { get; set; }

        public string BaseShape { get; set; }

        public string BaseShapeSize { get; set; }

        public int NoLayers { get; set; }

        public string LayerShape { get; set; }

        public string IcingType { get; set; }

        public string? Toppers { get; set; }
        
        public bool? IsReqested { get; set; } = false;

        public double? RequestedPrice { get; set; } = null;

        public ICollection<CakeLayerModel> CakeLayers { get; set; } = new List<CakeLayerModel>();

        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
