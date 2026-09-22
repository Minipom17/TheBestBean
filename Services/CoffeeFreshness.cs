using System.Text.Json;

namespace TheBestBean.Services
{
    public enum FreshPhase
    {
        Degas,
        Peak,
        Fade,
        Stale
    }

    public sealed class RoastedDrop
    {
        public DateTime RoastDate { get; init; }
        public int DaysAgo { get; init; }
        public string Profile { get; init; } = "Our roast";
        public decimal? WeightGrams { get; init; }
        public FreshPhase Phase { get; init; }
        public string PhaseLabel { get; init; } = "";
        public string PhaseHint { get; init; } = "";
    }

    public sealed class CoffeeLotStatus
    {
        public int CoffeeBeanId { get; init; }
        public string Name { get; init; } = "";
        public decimal GreenKg { get; init; }
        public decimal RoastedKg { get; init; }
        public string? Farmer { get; init; }
        public string? Process { get; init; }
        public string? Variety { get; init; }
        public string? Region { get; init; }
        public DateTime? HarvestedOn { get; init; }
        public DateTime? FermentedOn { get; init; }
        public DateTime? DriedOn { get; init; }
        public DateTime? ArrivedCuscoOn { get; init; }
        public IReadOnlyList<RoastedDrop> Roasts { get; init; } = Array.Empty<RoastedDrop>();

        public bool HasGreen => GreenKg >= 0.05m;
        public bool HasRoast => Roasts.Count > 0;

        public string CardLine
        {
            get
            {
                var latest = Roasts.FirstOrDefault();
                if (HasGreen && latest != null)
                {
                    return $"{FormatKg(GreenKg)} green · last roast {FormatKg(RoastedKg)} {latest.DaysAgo}d · {latest.Profile}";
                }

                if (HasGreen)
                {
                    return $"{FormatKg(GreenKg)} green · roast after you order";
                }

                if (latest != null)
                {
                    return $"Roasted {latest.DaysAgo}d ago · {latest.Profile} · {latest.PhaseLabel}";
                }

                return "Roast to order";
            }
        }

        public static string FormatKg(decimal kg)
        {
            return $"{Math.Round(kg, 1, MidpointRounding.AwayFromZero):0.0} kg";
        }

        public string SheetJson()
        {
            return JsonSerializer.Serialize(new
            {
                name = Name,
                farmer = Farmer,
                process = Process,
                variety = Variety,
                region = Region,
                warehouse = "Cusco",
                greenKg = FormatKg(GreenKg),
                roastedKg = FormatKg(RoastedKg),
                harvested = Iso(HarvestedOn),
                fermented = Iso(FermentedOn),
                dried = Iso(DriedOn),
                arrived = Iso(ArrivedCuscoOn),
                roasts = Roasts.Select(r => new
                {
                    date = r.RoastDate.ToString("yyyy-MM-dd"),
                    days = r.DaysAgo,
                    profile = r.Profile,
                    phase = r.PhaseLabel,
                    kg = r.WeightGrams.HasValue ? FormatKg(r.WeightGrams.Value / 1000m) : null
                })
            });
        }

        private static string? Iso(DateTime? value) =>
            value.HasValue ? value.Value.ToString("yyyy-MM-dd") : null;
    }

    public sealed class CoffeeLabBoard
    {
        public IReadOnlyList<CoffeeLotStatus> Lots { get; init; } = Array.Empty<CoffeeLotStatus>();
        public bool Compact { get; init; }
        public bool ShowTimeline { get; init; } = true;
        public bool ShowGrind { get; init; } = true;
        public bool ShowLede { get; init; } = true;
        public string Kicker { get; init; } = "Lab · live";
        public string Title { get; init; } = "What’s in the lab";
        public bool ShowLegend { get; init; }
        public bool ShowHowDetails { get; init; }
        public FreshPhase? Highlight { get; init; }
    }
}
