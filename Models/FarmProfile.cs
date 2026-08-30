using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    public class FarmProfile
    {
        public int Id { get; set; }

        [Required]
        public string FarmName { get; set; } = string.Empty;

        [Required]
        public string FarmerName { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        public string? GPSLatitude { get; set; }
        public string? GPSLongitude { get; set; }
        
        // Auto-populated from survey
        public int? RelatedSurveyId { get; set; }
        public FarmerSurvey? RelatedSurvey { get; set; }

        // Farm photos for carousel
        public string FarmPhotos { get; set; } = string.Empty; // JSON array of photo URLs
        
        // Farm story and description
        public string FarmStory { get; set; } = string.Empty;
        
        // Coffee information
        public string CoffeeVarieties { get; set; } = string.Empty;
        public string ProcessingMethod { get; set; } = string.Empty;
        public string FarmingPhilosophy { get; set; } = string.Empty;
        
        // Environmental info
        public string EnvironmentalFeatures { get; set; } = string.Empty;
        public string ClimateNotes { get; set; } = string.Empty;
        
        // Quality highlights
        public string QualityHighlights { get; set; } = string.Empty;
        public int? QualityRating { get; set; }
        
        // Additional content sections
        public string SustainabilityPractices { get; set; } = string.Empty;
        public string CommunityImpact { get; set; } = string.Empty;
        
        // Profile metadata
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
