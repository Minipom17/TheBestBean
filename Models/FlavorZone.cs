using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Collections.Generic;

namespace TheBestBean.Models
{
    public class FlavorZone
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Stored as JSON strings in the database
        public string MarkersJson { get; set; } = "[]";
        
        [NotMapped]
        public List<string> Markers
        {
            get => string.IsNullOrEmpty(MarkersJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(MarkersJson) ?? new List<string>();
            set => MarkersJson = JsonSerializer.Serialize(value);
        }

        public string Color { get; set; } = string.Empty;
        
        // BaryTarget stored as JSON for D3/Canvas interpolation
        public string BaryTargetJson { get; set; } = "{}";
        
        // Specs mapped to Range Sliders [Light, Dark], [Crisp, Heavy], etc.
        public string SpecsJson { get; set; } = "{}";

        // Recommendation section
        public string RecommendationTitle { get; set; } = string.Empty;
        public string RecommendationOrigin { get; set; } = string.Empty;
        public string RecommendationRoast { get; set; } = string.Empty;
        public string RecommendationDesc { get; set; } = string.Empty;
    }
}
