using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    public class CoffeeBean
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Flavor Profile")]
        public string FlavorProfile { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Flavor Profile (Spanish)")]
        public string FlavorProfileES { get; set; } = string.Empty;

        [Range(1, 10)]
        [Column(TypeName = "decimal(3, 1)")]
        public decimal Rating { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal ScaScore { get; set; } = 86.00m;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePriceUSD { get; set; } = 24.00m;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePricePEN { get; set; } = 90.00m;

        [Display(Name = "Processing Method")]
        public string ProcessingMethod { get; set; } = string.Empty; // e.g., Washed, Natural, Honey

        [StringLength(100)]
        public string Producer { get; set; } = string.Empty;

        [StringLength(100)]
        public string Variety { get; set; } = string.Empty;

        [StringLength(50)]
        public string Altitude { get; set; } = string.Empty;

        [StringLength(250)]
        public string ImageUrl { get; set; } = "/images/coffee-bean-marker.png";

        [StringLength(250)]
        public string ProducerImageUrl { get; set; } = string.Empty;

        public string ProducerDescription { get; set; } = string.Empty;
        public string ProducerDescriptionES { get; set; } = string.Empty;

        public string ProcessingDescription { get; set; } = string.Empty;
        public string ProcessingDescriptionES { get; set; } = string.Empty;

        [StringLength(250)]
        public string MapImageUrl { get; set; } = string.Empty;

        public string OriginDescription { get; set; } = string.Empty;
        public string OriginDescriptionES { get; set; } = string.Empty;

        // Foreign Keys for Relationships
        [Display(Name = "Farm")]
        public int CoffeeFarmId { get; set; }

        [Display(Name = "Region")]
        public int CoffeeRegionId { get; set; }

        [Display(Name = "Origin Country")]
        public int OriginCountryId { get; set; }

        // Navigation Properties (to load related data)
        public CoffeeFarm CoffeeFarm { get; set; } = default!;
        public CoffeeRegion CoffeeRegion { get; set; } = default!;
        public OriginCountry OriginCountry { get; set; } = default!;
    }
}
