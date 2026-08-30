using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TheBestBean.Models
{
    /// <summary>
    /// Represents a roasting batch with temperature curve data
    /// Stores the complete roasting profile including temperature readings and events
    /// </summary>
    public class RoastBatch
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Bean Inventory")]
        public int BeanInventoryId { get; set; }

        [Display(Name = "Roast Date")]
        [DataType(DataType.Date)]
        public DateTime RoastDate { get; set; } = DateTime.Now;

        [Display(Name = "Roast Time (seconds)")]
        public int? RoastTimeSeconds { get; set; }

        [Display(Name = "Green Weight (g)")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? GreenWeightGrams { get; set; }

        [Display(Name = "Roasted Weight (g)")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RoastedWeightGrams { get; set; }

        [Display(Name = "Weight Loss %")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? WeightLossPercent { get; set; }

        [Display(Name = "Development Time Ratio (DTR) %")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? DTRPercent { get; set; }

        [Display(Name = "First Crack Time (seconds)")]
        public int? FirstCrackTime { get; set; }

        [Display(Name = "Drop Time (seconds)")]
        public int? DropTime { get; set; }

        [Display(Name = "Turn Point Time (seconds)")]
        public int? TurnPointTime { get; set; }

        [Display(Name = "Dry End Time (seconds)")]
        public int? DryEndTime { get; set; }

        // Temperature curve data stored as JSON
        // Format: [{"time": 0, "temp": null, "ror": null}, ...]
        [Display(Name = "Temperature Data")]
        public string? TemperatureDataJson { get; set; }

        // Roasting events stored as JSON
        // Format: [{"type": "FC", "time": 300}, ...]
        [Display(Name = "Roasting Events")]
        public string? EventsJson { get; set; }

        [Display(Name = "Batch Notes")]
        [StringLength(2000)]
        public string? Notes { get; set; }

        [Display(Name = "Roast Level")]
        [StringLength(50)]
        public string? RoastLevel { get; set; } // Light, Medium, Dark, etc.

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual BeanInventory BeanInventory { get; set; } = default!;
        public virtual ICollection<CuppingScore> CuppingScores { get; set; } = new List<CuppingScore>();

        // Helper methods to work with JSON data
        public List<TemperaturePoint> GetTemperatureData()
        {
            if (string.IsNullOrEmpty(TemperatureDataJson))
                return new List<TemperaturePoint>();

            try
            {
                return JsonSerializer.Deserialize<List<TemperaturePoint>>(TemperatureDataJson) ?? new List<TemperaturePoint>();
            }
            catch
            {
                return new List<TemperaturePoint>();
            }
        }

        public void SetTemperatureData(List<TemperaturePoint> data)
        {
            TemperatureDataJson = JsonSerializer.Serialize(data);
        }

        public List<RoastEvent> GetEvents()
        {
            if (string.IsNullOrEmpty(EventsJson))
                return new List<RoastEvent>();

            try
            {
                return JsonSerializer.Deserialize<List<RoastEvent>>(EventsJson) ?? new List<RoastEvent>();
            }
            catch
            {
                return new List<RoastEvent>();
            }
        }

        public void SetEvents(List<RoastEvent> events)
        {
            EventsJson = JsonSerializer.Serialize(events);
        }
    }

    // Helper classes for temperature data
    public class TemperaturePoint
    {
        public int Time { get; set; }
        public double? Temp { get; set; }
        public double? RoR { get; set; } // Rate of Rise
    }

    public class RoastEvent
    {
        public string Type { get; set; } = string.Empty; // TP, Dry, FC, DROP
        public int Time { get; set; }
    }
}


