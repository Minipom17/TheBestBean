using System.ComponentModel.DataAnnotations;

namespace TheBestBean.Models
{
    public class CoffeeRegion
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [Required]
        [Display(Name = "Region Name")]
        public string Name { get; set; } = string.Empty;
    }
}
