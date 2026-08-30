using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    /// <summary>
    /// Represents raw coffee bean inventory (Yamakafi) from farmers
    /// Used for tracking green coffee stock before roasting
    /// </summary>
    public class BeanInventory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Bean Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Country of Origin")]
        public string? Country { get; set; }

        [StringLength(150)]
        [Display(Name = "Farmer/Producer Name")]
        public string? Farmer { get; set; }

        [StringLength(50)]
        [Display(Name = "Processing Method")]
        public string? Process { get; set; } // Washed, Natural, Honey

        [Display(Name = "Weight (kg)")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalKg { get; set; }

        [Display(Name = "Cost per kg")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? CostPerKg { get; set; }

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Lot Number")]
        [StringLength(100)]
        public string? LotNumber { get; set; }

        [Display(Name = "Farm/Origin Region")]
        [StringLength(200)]
        public string? Region { get; set; }

        [Display(Name = "Variety")]
        [StringLength(100)]
        public string? Variety { get; set; } // Typica, Bourbon, Caturra, etc.

        [Display(Name = "Elevation (m)")]
        public int? ElevationMeters { get; set; }

        [Display(Name = "Harvest Year")]
        public int? HarvestYear { get; set; }

        [Display(Name = "Notes")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Has Certificates")]
        public bool HasCertificates { get; set; } = false;

        [Display(Name = "Certificate File Path")]
        [StringLength(500)]
        public string? CertificateFilePath { get; set; }

        // Navigation properties
        public virtual ICollection<RoastBatch> RoastBatches { get; set; } = new List<RoastBatch>();
    }
}


