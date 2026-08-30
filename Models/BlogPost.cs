using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string TitleES { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty; // Markdown or HTML content

        public string ContentES { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = false;
        
        public DateTime? PublishedAt { get; set; }
    }
}
