namespace TheBestBean.Services
{
    /// <summary>
    /// Local Yape/Plin soles vs international card USD.
    /// Yape uses the listed PEN price (or 3.75× USD). Card is shown in USD
    /// and as a soles equivalent at a higher rate so bank FX + card fees are obvious.
    /// </summary>
    public static class LocalPricing
    {
        public const string WorkshopVisaFeeNote = "VISA 5%";

        public const decimal YapeRate = 3.75m;
        public const decimal CardRate = 4.10m;

        public static decimal YapeSoles(decimal usd, decimal listedPen = 0)
        {
            if (listedPen > 0)
            {
                return Math.Round(listedPen, 0);
            }

            return Math.Round(usd * YapeRate, 0);
        }

        public static decimal CardSoles(decimal usd) => Math.Round(usd * CardRate, 0);

        public static decimal YapeSavings(decimal usd, decimal listedPen = 0)
        {
            var save = CardSoles(usd) - YapeSoles(usd, listedPen);
            return save > 0 ? save : 0;
        }

        /// <summary>
        /// 100 g Yape price in soles from SCA. 88 pts = S/30; CoE lots sit above; lower scores step down.
        /// Larger bags use the same bulk ratios as the 88-pt ladder (200 g S/50, 500 g S/120, 1 kg S/200).
        /// </summary>
        public static decimal CoffeeHundredGramsPen(decimal sca) => sca switch
        {
            >= 90m => 42m,
            >= 88m => 30m,
            >= 87.5m => 28m,
            >= 87m => 26m,
            >= 86.5m => 24m,
            >= 86m => 22m,
            _ => 18m
        };

        public static decimal CoffeeBagMultiplier(string? weight) => (weight ?? "100g").Trim().ToLowerInvariant() switch
        {
            "200g" => 50m / 30m,
            "500g" => 4m,
            "1kg" or "1000g" => 200m / 30m,
            _ => 1m
        };

        public static decimal CoffeeBagPen(decimal sca, string? weight = "100g") =>
            Math.Round(CoffeeHundredGramsPen(sca) * CoffeeBagMultiplier(weight), 0);

        public static decimal CoffeeBagUsd(decimal sca, string? weight = "100g") =>
            Math.Round(CoffeeBagPen(sca, weight) / YapeRate, 2);
    }
}
