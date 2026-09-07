using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TheBestBean.Models
{
    public class Experience
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string TitleES { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "Urban Workshops"; // "Urban Workshops" or "Expedition"

        [Required]
        [StringLength(50)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Month { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Difficulty { get; set; } = string.Empty; // "Beginner", "Intermediate", "Advanced"

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string DescriptionES { get; set; } = string.Empty;

        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, 10000)]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? Tag { get; set; } // E.g. "GUEST EVENT"

        /// <summary>
        /// Listing rank. Lower comes first. Featured lab = 0, San Blas pour-over = 10, Latte Art last among urban workshops.
        /// </summary>
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; } = 100;

        [StringLength(50)]
        public string Duration { get; set; } = string.Empty;

        public string LongDescription { get; set; } = string.Empty;
        public string LongDescriptionES { get; set; } = string.Empty;

        // JSON backing fields for SQLite support
        [Column("Syllabus")]
        public string SyllabusJson { get; set; } = "[]";

        [NotMapped]
        public List<string> Syllabus
        {
            get => JsonSerializer.Deserialize<List<string>>(SyllabusJson) ?? new List<string>();
            set => SyllabusJson = JsonSerializer.Serialize(value);
        }

        [Column("ProvidedEquipment")]
        public string ProvidedEquipmentJson { get; set; } = "[]";

        [NotMapped]
        public List<string> ProvidedEquipment
        {
            get => JsonSerializer.Deserialize<List<string>>(ProvidedEquipmentJson) ?? new List<string>();
            set => ProvidedEquipmentJson = JsonSerializer.Serialize(value);
        }

        [Column("RequiredGear")]
        public string RequiredGearJson { get; set; } = "[]";

        [NotMapped]
        public List<string> RequiredGear
        {
            get => JsonSerializer.Deserialize<List<string>>(RequiredGearJson) ?? new List<string>();
            set => RequiredGearJson = JsonSerializer.Serialize(value);
        }

        [Column("GalleryImages")]
        public string GalleryImagesJson { get; set; } = "[]";

        [NotMapped]
        public List<string> GalleryImages
        {
            get => JsonSerializer.Deserialize<List<string>>(GalleryImagesJson) ?? new List<string>();
            set => GalleryImagesJson = JsonSerializer.Serialize(value);
        }

        public bool IsFeaturedWorkshop =>
            string.Equals(Tag, "FEATURED", StringComparison.OrdinalIgnoreCase);

        public bool IsSanBlasPourOver =>
            (Title ?? string.Empty).Contains("San Blas", StringComparison.OrdinalIgnoreCase);

        public bool IsCuscoCoffeeLab =>
            (Title ?? string.Empty).Contains("Coffee Lab", StringComparison.OrdinalIgnoreCase)
            || (Title ?? string.Empty).Contains("Coffee Laboratory", StringComparison.OrdinalIgnoreCase);

        public string LocalizedTitle(bool spanish) =>
            spanish && !string.IsNullOrWhiteSpace(TitleES) ? TitleES : Title;

        public string LocalizedDescription(bool spanish) =>
            spanish && !string.IsNullOrWhiteSpace(DescriptionES) ? DescriptionES : Description;

        public string LocalizedLongDescription(bool spanish) =>
            spanish && !string.IsNullOrWhiteSpace(LongDescriptionES) ? LongDescriptionES : LongDescription;
    }
}
