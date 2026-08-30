using System.ComponentModel.DataAnnotations;

namespace TheBestBean.Models
{
    public class SiteContent
    {
        [Key]
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Page { get; set; } = string.Empty;
    }
}
