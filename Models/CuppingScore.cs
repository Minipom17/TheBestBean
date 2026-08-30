using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    /// <summary>
    /// Represents SCA (Specialty Coffee Association) cupping scorecard data
    /// Stores detailed cupping evaluation for a roast batch
    /// </summary>
    public class CuppingScore
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Roast Batch")]
        public int RoastBatchId { get; set; }

        [Display(Name = "Cupper Name")]
        [StringLength(150)]
        public string? CupperName { get; set; }

        [Display(Name = "Cupping Date")]
        [DataType(DataType.Date)]
        public DateTime CuppingDate { get; set; } = DateTime.Now;

        [Display(Name = "Lot Number")]
        [StringLength(100)]
        public string? LotNumber { get; set; }

        // Roast level squares (3 squares for light/medium/dark)
        [Display(Name = "Roast Level")]
        [StringLength(20)]
        public string? RoastLevel { get; set; } // JSON: [true, false, false] for selected squares

        // Fragrance scores
        [Display(Name = "Fragrance Dry")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? FragranceDry { get; set; }

        [Display(Name = "Fragrance Break")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? FragranceBreak { get; set; }

        [Display(Name = "Fragrance Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? FragranceScore { get; set; }

        // Flavor scores
        [Display(Name = "Flavor Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? FlavorScore { get; set; }

        [Display(Name = "Aftertaste Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? AftertasteScore { get; set; }

        // Acidity
        [Display(Name = "Acidity Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? AcidityScore { get; set; }

        [Display(Name = "Acidity Intensity")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? AcidityIntensity { get; set; } // 0-10 scale for High/Low

        // Body
        [Display(Name = "Body Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? BodyScore { get; set; }

        [Display(Name = "Body Intensity")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? BodyIntensity { get; set; } // 0-10 scale for Heavy/Thin

        // Balance & Uniformity
        [Display(Name = "Balance Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? BalanceScore { get; set; }

        [Display(Name = "Uniformity Checks")]
        public int? UniformityChecks { get; set; } // 0-5 checked squares

        // Sweetness & Clean Cup
        [Display(Name = "Sweetness Checks")]
        public int? SweetnessChecks { get; set; } // 0-5 checked squares

        [Display(Name = "Clean Cup Checks")]
        public int? CleanCupChecks { get; set; } // 0-5 checked squares

        // Overall & Defects
        [Display(Name = "Overall Score")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? OverallScore { get; set; }

        [Display(Name = "Defect Cups")]
        public int? DefectCups { get; set; }

        [Display(Name = "Final Score")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? FinalScore { get; set; }

        [Display(Name = "Cupping Notes")]
        [StringLength(2000)]
        public string? Notes { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual RoastBatch RoastBatch { get; set; } = default!;
    }
}


