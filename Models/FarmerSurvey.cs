using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    public class FarmerSurvey
    {
        public int Id { get; set; }

        // 🔑 Core Farm Identification
        [Required(ErrorMessage = "Farm name is required")]
        [StringLength(200, ErrorMessage = "Farm name cannot exceed 200 characters")]
        [Display(Name = "Farm/Cooperative Name")]
        public string FarmName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Farmer's name is required")]
        [StringLength(150, ErrorMessage = "Farmer's name cannot exceed 150 characters")]
        [Display(Name = "Farmer's Name/Contact")]
        public string FarmerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required")]
        [StringLength(100, ErrorMessage = "Region cannot exceed 100 characters")]
        [Display(Name = "Region/Province/District")]
        public string Region { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "GPS coordinates cannot exceed 50 characters")]
        [Display(Name = "GPS Coordinates (lat/long)")]
        public string GpsCoordinates { get; set; } = string.Empty;

        [Required(ErrorMessage = "Survey date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Survey")]
        public DateTime SurveyDate { get; set; } = DateTime.Today;

        // Optional single photo for the survey
        [StringLength(500)]
        public string? SurveyPhotoPath { get; set; }

        // 🌍 Site & Environmental Conditions
        [Display(Name = "Elevation (m above sea level)")]
        [Range(0, 4000, ErrorMessage = "Elevation must be between 0 and 4000 meters")]
        public int? ElevationMeters { get; set; }

        [Display(Name = "Average Slope (%)")]
        [StringLength(20, ErrorMessage = "Slope description cannot exceed 20 characters")]
        public string SlopeType { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Aspect cannot exceed 50 characters")]
        [Display(Name = "Aspect/Orientation")]
        public string Aspect { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Temperature range cannot exceed 50 characters")]
        [Display(Name = "Average Temperature Range")]
        public string TemperatureRange { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Rainfall description cannot exceed 100 characters")]
        [Display(Name = "Rainfall Patterns")]
        public string RainfallPatterns { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Water source description cannot exceed 100 characters")]
        [Display(Name = "Water Source")]
        public string WaterSource { get; set; } = string.Empty;

        // 🌱 Farm Characteristics
        [Display(Name = "Total Farm Size (hectares)")]
        [Range(0.1, 1000, ErrorMessage = "Farm size must be between 0.1 and 1000 hectares")]
        public decimal? TotalFarmSizeHa { get; set; }

        [Display(Name = "Coffee Planted Area (hectares)")]
        [Range(0.1, 1000, ErrorMessage = "Coffee area must be between 0.1 and 1000 hectares")]
        public decimal? CoffeePlantedAreaHa { get; set; }

        [StringLength(200, ErrorMessage = "Varieties cannot exceed 200 characters")]
        [Display(Name = "Coffee Varieties Grown")]
        public string VarietiesGrown { get; set; } = string.Empty;

        [Display(Name = "Planting Density (trees per hectare)")]
        [Range(100, 10000, ErrorMessage = "Planting density must be between 100 and 10000 trees per hectare")]
        public int? PlantingDensityTreesHa { get; set; }

        [StringLength(100, ErrorMessage = "Shade tree description cannot exceed 100 characters")]
        [Display(Name = "Shade Trees (species & % cover)")]
        public string ShadeTrees { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Other crops description cannot exceed 200 characters")]
        [Display(Name = "Other Crops")]
        public string OtherCrops { get; set; } = string.Empty;

        [Display(Name = "Intercropping/Agroforestry")]
        public bool HasIntercropping { get; set; }

        [StringLength(200, ErrorMessage = "Intercropping details cannot exceed 200 characters")]
        [Display(Name = "Intercropping Details")]
        public string IntercroppingDetails { get; set; } = string.Empty;

        // 🌾 Soil & Ground Conditions
        [StringLength(100, ErrorMessage = "Soil type cannot exceed 100 characters")]
        [Display(Name = "Soil Type")]
        public string SoilType { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Soil depth description cannot exceed 100 characters")]
        [Display(Name = "Soil Depth/Rockiness")]
        public string SoilDepth { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Drainage description cannot exceed 100 characters")]
        [Display(Name = "Drainage Conditions")]
        public string DrainageConditions { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Erosion risks cannot exceed 100 characters")]
        [Display(Name = "Erosion Risks")]
        public string ErosionRisks { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Fertility management description cannot exceed 200 characters")]
        [Display(Name = "Soil Fertility Management")]
        public string SoilFertilityManagement { get; set; } = string.Empty;

        // ☕ Coffee Management Practices
        [Display(Name = "Farm Age (year coffee planted)")]
        [Range(1800, 2030, ErrorMessage = "Planting year must be realistic")]
        public int? CoffeePlantingYear { get; set; }

        [StringLength(100, ErrorMessage = "Pruning method cannot exceed 100 characters")]
        [Display(Name = "Pruning Method")]
        public string PruningMethod { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Fertilization cannot exceed 100 characters")]
        [Display(Name = "Fertilization")]
        public string FertilizationType { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Pest & disease description cannot exceed 200 characters")]
        [Display(Name = "Pest & Disease Presence")]
        public string PestDiseasePresence { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Harvesting method cannot exceed 100 characters")]
        [Display(Name = "Harvesting Method")]
        public string HarvestingMethod { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Processing method cannot exceed 100 characters")]
        [Display(Name = "Processing Method")]
        public string ProcessingMethod { get; set; } = string.Empty;

        // 📦 Post-Harvest & Storage
        [StringLength(100, ErrorMessage = "Fermentation description cannot exceed 100 characters")]
        [Display(Name = "Fermentation Type & Duration")]
        public string FermentationType { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Drying method cannot exceed 100 characters")]
        [Display(Name = "Drying Method")]
        public string DryingMethod { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Drying time cannot exceed 50 characters")]
        [Display(Name = "Average Drying Time")]
        public string DryingTime { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Storage conditions cannot exceed 100 characters")]
        [Display(Name = "Storage Facility Conditions")]
        public string StorageConditions { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Traceability system cannot exceed 100 characters")]
        [Display(Name = "Traceability System")]
        public string TraceabilitySystem { get; set; } = string.Empty;

        // 👥 Socio-Economic Info
        [StringLength(100, ErrorMessage = "Labor system cannot exceed 100 characters")]
        [Display(Name = "Labor System")]
        public string LaborSystem { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Cooperative membership cannot exceed 200 characters")]
        [Display(Name = "Cooperative Membership")]
        public string CooperativeMembership { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Certifications cannot exceed 200 characters")]
        [Display(Name = "Certifications")]
        public string Certifications { get; set; } = string.Empty;

        [Display(Name = "Annual Production (kg green coffee)")]
        [Range(0, 1000000, ErrorMessage = "Production must be realistic")]
        public decimal? AnnualProductionKg { get; set; }

        // 📝 Notes & Observations
        [StringLength(1000, ErrorMessage = "Unique qualities cannot exceed 1000 characters")]
        [Display(Name = "Unique Qualities of the Farm")]
        public string UniqueQualities { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Challenges cannot exceed 1000 characters")]
        [Display(Name = "Challenges Mentioned")]
        public string ChallengesMentioned { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Photographs description cannot exceed 500 characters")]
        [Display(Name = "Photographs Taken")]
        public string PhotographsTaken { get; set; } = string.Empty;

        // Additional fields for internal use
        [Display(Name = "Survey Notes")]
        public string SurveyNotes { get; set; } = string.Empty;

        [Display(Name = "Follow-up Required")]
        public bool FollowUpRequired { get; set; }

        [Display(Name = "Quality Rating (1-10)")]
        [Range(1, 10, ErrorMessage = "Quality rating must be between 1 and 10")]
        public int? QualityRating { get; set; }

        // Computed property for farm efficiency
        [NotMapped]
        public decimal? CoffeeAreaPercentage 
        { 
            get 
            {
                if (TotalFarmSizeHa.HasValue && CoffeePlantedAreaHa.HasValue && TotalFarmSizeHa.Value > 0)
                {
                    return (CoffeePlantedAreaHa.Value / TotalFarmSizeHa.Value) * 100;
                }
                return null;
            }
        }

        // Computed property for farm age
        [NotMapped]
        public int? FarmAgeYears
        {
            get
            {
                if (CoffeePlantingYear.HasValue)
                {
                    return DateTime.Now.Year - CoffeePlantingYear.Value;
                }
                return null;
            }
        }
    }
}
