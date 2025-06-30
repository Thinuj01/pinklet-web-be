using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pinklet.Models
{
    public class Cake
    {
        [Key]
        public int Id { get; set; }
        public string CakeCode { get; set; }
        public string CakeName { get; set; }
        public string CakeCategory { get; set; }
        public string CakeTags { get; set; }
        public double CakePrice { get; set; }
        public int CakeRating { get; set; }
        public string CakeDescription { get; set; }
        public string? CakeImageLink1 { get; set; }
        public string? CakeImageLink2 { get; set; }
        public string? CakeImageLink3 { get; set; }
        public string? CakeImageLink4 { get; set; }
        public string? Cake3dModelLink { get; set; }

        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
