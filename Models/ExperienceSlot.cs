using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBestBean.Models
{
    public class ExperienceSlot
    {
        [Key]
        public int Id { get; set; }
        public int ExperienceId { get; set; }
        public Experience? Experience { get; set; }
        public DateTime StartAt { get; set; }
        public int Capacity { get; set; } = 6;
        public int BookedCount { get; set; }
        public bool IsCancelled { get; set; }
        /// <summary>Admin reopen: ignore the normal booking cutoff (morning day-before / 4h lead).</summary>
        public bool BypassCutoff { get; set; }

        [NotMapped]
        public int Remaining => Math.Max(0, Capacity - BookedCount);
    }
}
