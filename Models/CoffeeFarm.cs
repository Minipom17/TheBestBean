using System.ComponentModel.DataAnnotations;

namespace TheBestBean.Models
{
    public class CoffeeFarm
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Display(Name = "Farm Name")]
        public string Name { get; set; } = string.Empty;
    }
}
