namespace TheBestBean.Services
{
    /// <summary>
    /// Local Yape/Plin soles vs listed USD and the Perú card soles price.
    /// </summary>
    public static class LocalPricing
    {
        /// <summary>
        /// Two posted prices, shown from the first screen:
        /// Yape/Plin = local soles. Perú cards (Culqi) = 5% above Yape (shop card price).
        /// PayPal USD/CAD = listed USD / CAD FX with no extra %. PayPal forbids a PayPal
        /// surcharge; we do not add Canada’s 2.4% Visa cap on top of PayPal.
        /// </summary>
        public const decimal PeruCardRate = 0.05m;
        public const decimal PeruCardSurchargeRate = PeruCardRate;

        public const decimal YapeRate = 3.40m;
        public const decimal CardRate = 4.10m;
        /// <summary>CAD charged on PayPal per 1 USD list price. Not tax — FX only.</summary>
        public const decimal CadRate = 1.38m;

        public static decimal Cad(decimal usd) => Math.Round(usd * CadRate, 2);

        public static decimal CardSurchargeRateFor(string currency) =>
            (currency ?? "").Trim().ToUpperInvariant() switch
            {
                "PEN" => PeruCardRate,
                _ => 0m
            };

        public static decimal CardSurcharge(decimal amount, string currency) =>
            Math.Round(amount * CardSurchargeRateFor(currency), 2, MidpointRounding.AwayFromZero);

        public static decimal WithCardSurcharge(decimal amount, string currency) =>
            amount + CardSurcharge(amount, currency);

        public static string CardSurchargeLabel(string currency) =>
            (currency ?? "").Trim().ToUpperInvariant() switch
            {
                "PEN" => "Card price · Perú",
                _ => "Listed price"
            };

        /// <summary>Perú card (Culqi) soles: Yape price plus 5%, whole soles.</summary>
        public static decimal CulqiSoles(decimal usd, decimal listedPen = 0) =>
            Math.Round(YapeSoles(usd, listedPen) * (1m + PeruCardRate), 0, MidpointRounding.AwayFromZero);

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
