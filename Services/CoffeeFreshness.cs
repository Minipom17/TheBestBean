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
            // Always keep a decimal for uniformity (0.0 kg). Use hundredths only when needed (0.25 kg).
            var toTwo = Math.Round(kg, 2, MidpointRounding.AwayFromZero);
            var toOne = Math.Round(kg, 1, MidpointRounding.AwayFromZero);
            if (toTwo == toOne)
            {
                return $"{toOne:0.0} kg";
            }

            return $"{toTwo:0.00} kg";
        }
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
        public FreshPhase? Highlight { get; init; }
    }
}
