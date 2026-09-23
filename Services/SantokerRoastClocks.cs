using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public static class SantokerRoastClocks
    {
        private sealed class Curve
        {
            public string File { get; set; } = "";
            public List<string> Match { get; set; } = new();
            public DateTime? RoastDate { get; set; }
            public decimal? ChargeGrams { get; set; }
            public string? RoastLevel { get; set; }
            public int? DurationSeconds { get; set; }
            public int? DropSeconds { get; set; }
            public double? DropTempC { get; set; }
            public int? TurnaroundSeconds { get; set; }
            public int? DryEndSeconds { get; set; }
            public int? FirstCrackSeconds { get; set; }
            public decimal? DtrPercent { get; set; }
        }

        public static async Task ApplyAsync(TheBestBeanContext db, string contentRoot, CancellationToken ct = default)
        {
            var path = Path.Combine(contentRoot, "santoker", "roasts.json");
            if (!File.Exists(path))
            {
                return;
            }

            var curves = JsonSerializer.Deserialize<List<Curve>>(await File.ReadAllTextAsync(path, ct), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (curves == null || curves.Count == 0)
            {
                return;
            }

            var beans = await db.CoffeeBean.ToListAsync(ct);
            var inventory = await db.BeanInventories
                .Include(b => b.RoastBatches)
                .Where(b => b.IsActive)
                .ToListAsync(ct);

            foreach (var curve in curves)
            {
                var matched = beans.Where(b => Hits(b, curve.Match)).ToList();
                if (matched.Count == 0)
                {
                    continue;
                }

                foreach (var bean in matched)
                {
                var lots = inventory.Where(inv =>
                    inv.CoffeeBeanId == bean.Id || CoffeeFreshnessService.NamesMatch(inv.Name, bean.Name)).ToList();
                if (lots.Count == 0)
                {
                    var created = new BeanInventory
                    {
                        Name = bean.Name,
                        CoffeeBeanId = bean.Id,
                        Country = "Peru",
                        Process = bean.ProcessingMethod,
                        Variety = bean.Variety,
                        Farmer = bean.Producer,
                        TotalKg = 0,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };
                    db.BeanInventories.Add(created);
                    inventory.Add(created);
                    lots.Add(created);
                }

                    foreach (var lot in lots)
                    {
                        ApplyToLot(db, lot, curve);
                    }
                }
            }

            await db.SaveChangesAsync(ct);
        }

        private static void ApplyToLot(TheBestBeanContext db, BeanInventory lot, Curve curve)
        {
            var roast = lot.RoastBatches.FirstOrDefault(r =>
                (r.Notes ?? "").StartsWith("Santoker GO", StringComparison.OrdinalIgnoreCase));
            if (roast == null)
            {
                roast = new RoastBatch
                {
                    BeanInventory = lot,
                    CreatedDate = DateTime.UtcNow
                };
                db.RoastBatches.Add(roast);
                lot.RoastBatches.Add(roast);
            }

            roast.RoastDate = curve.RoastDate.HasValue
                ? DateTime.SpecifyKind(curve.RoastDate.Value.Date, DateTimeKind.Utc)
                : roast.RoastDate;
            roast.RoastTimeSeconds = curve.DurationSeconds;
            roast.DropTime = curve.DropSeconds ?? curve.DurationSeconds;
            roast.TurnPointTime = curve.TurnaroundSeconds;
            roast.DryEndTime = curve.DryEndSeconds;
            roast.FirstCrackTime = curve.FirstCrackSeconds;
            roast.DTRPercent = curve.DtrPercent;
            roast.RoastLevel = string.IsNullOrWhiteSpace(curve.RoastLevel) ? roast.RoastLevel : curve.RoastLevel;
            roast.Notes = $"Santoker GO · {curve.File}";
            if (curve.ChargeGrams is > 0)
            {
                roast.GreenWeightGrams = curve.ChargeGrams;
                roast.RoastedWeightGrams = Math.Round(curve.ChargeGrams.Value * 0.86m, 0);
                roast.WeightLossPercent = 14m;
            }

            if (curve.DropTempC.HasValue && roast.DropTime is int dropAt)
            {
                roast.SetTemperatureData(new List<TemperaturePoint>
                {
                    new() { Time = dropAt, Temp = curve.DropTempC }
                });
            }
        }

        private static bool Hits(CoffeeBean bean, IEnumerable<string> needles)
        {
            foreach (var needle in needles)
            {
                if (string.IsNullOrWhiteSpace(needle))
                {
                    continue;
                }

                if ((bean.Name ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase)
                    || (bean.Variety ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
