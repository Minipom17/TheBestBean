using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public class CoffeeFreshnessService
    {
        private readonly TheBestBeanContext _db;

        public CoffeeFreshnessService(TheBestBeanContext db)
        {
            _db = db;
        }

        public static FreshPhase PhaseFor(int daysAgo)
        {
            if (daysAgo <= 5) return FreshPhase.Degas;
            if (daysAgo <= 42) return FreshPhase.Peak;
            if (daysAgo <= 90) return FreshPhase.Fade;
            return FreshPhase.Stale;
        }

        public static string PhaseLabel(FreshPhase phase) => phase switch
        {
            FreshPhase.Degas => "Degassing",
            FreshPhase.Peak => "Peak",
            FreshPhase.Fade => "Fading",
            _ => "Stale"
        };

        public static string PhaseHint(FreshPhase phase) => phase switch
        {
            FreshPhase.Degas => "0–5 days. Usually too fresh — sour notes or uneven brewing. Bloom on pour-over.",
            FreshPhase.Peak => "1–6 weeks. Excess gas is gone; origin notes and aroma are still loud.",
            FreshPhase.Fade => "6 weeks–3 months. Fruit and acidity start to dull into generic coffee.",
            _ => "3–9+ months. Safe to drink, but flat, papery, or woody."
        };

        public static string PhaseWindow(FreshPhase phase) => phase switch
        {
            FreshPhase.Degas => "0–5 days",
            FreshPhase.Peak => "1–6 weeks",
            FreshPhase.Fade => "6 wks–3 mo",
            _ => "3–9+ mo"
        };

        public async Task<IReadOnlyList<CoffeeLotStatus>> GetLotsAsync(IEnumerable<int>? coffeeBeanIds = null, CancellationToken ct = default)
        {
            var filter = coffeeBeanIds?.Where(id => id > 0).Distinct().ToHashSet();
            var shop = await _db.CoffeeBean.AsNoTracking()
                .Include(b => b.CoffeeRegion)
                .ToListAsync(ct);

            var inventory = await _db.BeanInventories.AsNoTracking()
                .Where(b => b.IsActive)
                .Include(b => b.RoastBatches)
                .ToListAsync(ct);

            var today = DateTime.UtcNow.Date;
            var lots = new List<CoffeeLotStatus>();

            foreach (var bean in shop)
            {
                if (filter != null && !filter.Contains(bean.Id))
                {
                    continue;
                }

                var matches = inventory.Where(inv => Matches(inv, bean.Id, bean.Name)).ToList();
                var green = matches.Sum(m => m.TotalKg);
                var roastedKg = matches
                    .SelectMany(m => m.RoastBatches)
                    .Sum(r => (r.RoastedWeightGrams ?? 0m) / 1000m);
                var drops = matches
                    .SelectMany(m => m.RoastBatches)
                    .OrderByDescending(r => r.RoastDate)
                    .Take(4)
                    .Select(r => ToDrop(r, today))
                    .ToList();
                var primary = matches
                    .OrderByDescending(m => m.ArrivedCuscoOn ?? DateTime.MinValue)
                    .ThenByDescending(m => m.CreatedDate)
                    .FirstOrDefault();
                var title = CoffeeCardTitle.From(bean);
                var display = CoffeeCardTitle.Line(title);

                lots.Add(new CoffeeLotStatus
                {
                    CoffeeBeanId = bean.Id,
                    Name = string.IsNullOrWhiteSpace(display) ? bean.Name : display,
                    GreenKg = green,
                    RoastedKg = roastedKg,
                    Farmer = primary?.Farmer ?? title.Farmer,
                    Process = primary?.Process,
                    Variety = title.Variety,
                    Region = title.Location,
                    HarvestedOn = primary?.HarvestedOn,
                    FermentedOn = primary?.FermentedOn,
                    DriedOn = primary?.DriedOn,
                    ArrivedCuscoOn = primary?.ArrivedCuscoOn,
                    Roasts = drops
                });
            }

            if (filter == null)
            {
                lots = LiveFeed(lots).ToList();
            }

            return lots;
        }

        public static IReadOnlyList<CoffeeLotStatus> LiveFeed(IEnumerable<CoffeeLotStatus> lots)
        {
            return lots
                .Where(l => l.HasGreen || l.RoastedKg >= 0.05m)
                .GroupBy(l => l.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => g
                    .OrderByDescending(l => l.GreenKg)
                    .ThenByDescending(l => l.RoastedKg)
                    .First())
                .OrderByDescending(l => l.HasGreen)
                .ThenByDescending(l => l.GreenKg)
                .ThenBy(l => l.Name)
                .ToList();
        }

        public async Task<CoffeeLotStatus?> GetLotAsync(int coffeeBeanId, CancellationToken ct = default)
        {
            var lots = await GetLotsAsync(new[] { coffeeBeanId }, ct);
            return lots.FirstOrDefault();
        }

        public async Task<CoffeeLabBoard> BoardForCartAsync(IEnumerable<CartItem> cart, CancellationToken ct = default)
        {
            var ids = cart.Where(i => RoastProfiles.IsCoffeeProduct(i.ProductType)).Select(i => i.ProductId).ToList();
            var lots = ids.Count == 0
                ? Array.Empty<CoffeeLotStatus>()
                : await GetLotsAsync(ids, ct);
            var highlight = lots.SelectMany(l => l.Roasts).FirstOrDefault()?.Phase;
            return new CoffeeLabBoard
            {
                Lots = lots,
                Compact = true,
                ShowTimeline = true,
                ShowGrind = true,
                Kicker = "Roast · rest · ship",
                Title = "This bag",
                Highlight = highlight
            };
        }

        public async Task<CoffeeLabBoard> LiveBoardAsync(CancellationToken ct = default)
        {
            var lots = await GetLotsAsync(null, ct);
            return new CoffeeLabBoard
            {
                Lots = lots,
                Compact = false,
                ShowTimeline = true,
                ShowGrind = true,
                Kicker = "Lab · live",
                Title = "What’s in the lab",
                Highlight = lots.SelectMany(l => l.Roasts).FirstOrDefault()?.Phase
            };
        }

        private static RoastedDrop ToDrop(RoastBatch roast, DateTime today)
        {
            var roastDay = roast.RoastDate.Date;
            var days = Math.Max(0, (today - roastDay).Days);
            var phase = PhaseFor(days);
            return new RoastedDrop
            {
                RoastDate = roastDay,
                DaysAgo = days,
                Profile = RoastProfiles.Label(roast.RoastLevel),
                WeightGrams = roast.RoastedWeightGrams,
                Phase = phase,
                PhaseLabel = PhaseLabel(phase),
                PhaseHint = PhaseHint(phase)
            };
        }

        private static bool Matches(BeanInventory inv, int coffeeBeanId, string coffeeName)
        {
            if (inv.CoffeeBeanId == coffeeBeanId)
            {
                return true;
            }

            return NamesMatch(inv.Name, coffeeName);
        }

        internal static bool NamesMatch(string? a, string? b)
        {
            var left = Norm(a);
            var right = Norm(b);
            if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
            {
                return false;
            }

            return left == right || left.Contains(right) || right.Contains(left);
        }

        private static string Norm(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            var chars = value.ToLowerInvariant()
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                .ToArray();
            return string.Join(' ', new string(chars).Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
