using System.ComponentModel.DataAnnotations;

namespace TheBestBean.Models
{
    public class OriginCountry
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        [Display(Name = "Country Name")]
        public string Name { get; set; } = string.Empty;
    }
}
