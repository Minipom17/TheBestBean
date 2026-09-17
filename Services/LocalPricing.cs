namespace TheBestBean.Services
{
    /// <summary>
    /// Local Yape/Plin soles vs international card USD.
    /// Yape uses the listed PEN price (or ~3.4× USD, near mid-market). Card is shown in USD
    /// and as a soles equivalent at a higher rate so bank FX + card fees are obvious.
    /// </summary>
    public static class LocalPricing
    {
        /// <summary>
        /// Card surcharge: Culqi PEN is Perú 5%. PayPal USD and CAD run on the Canadian
        /// account, so both use Visa Canada’s 2.4% cap — not the US merchant 3% cap.
        /// Yape is not a card — no surcharge. Disclose on the method they pick, before they pay.
        /// </summary>
        public const string WorkshopVisaFeeNote = "Visa 5% Perú · 2.4% worldwide";
        public const decimal PeruCardSurchargeRate = 0.05m;
        public const decimal CanadaCardSurchargeRate = 0.024m;
        public const decimal UsdCardSurchargeRate = CanadaCardSurchargeRate;

        public const decimal YapeRate = 3.40m;
        public const decimal CardRate = 4.10m;
        /// <summary>CAD charged on PayPal per 1 USD list price. Not tax — FX only.</summary>
        public const decimal CadRate = 1.38m;

        public static decimal Cad(decimal usd) => Math.Round(usd * CadRate, 2);

        public static decimal CardSurchargeRateFor(string currency) =>
            (currency ?? "").Trim().ToUpperInvariant() switch
            {
                "CAD" => CanadaCardSurchargeRate,
                "PEN" => PeruCardSurchargeRate,
                _ => UsdCardSurchargeRate
            };

        public static decimal CardSurcharge(decimal amount, string currency) =>
            Math.Round(amount * CardSurchargeRateFor(currency), 2, MidpointRounding.AwayFromZero);

        public static decimal WithCardSurcharge(decimal amount, string currency) =>
            amount + CardSurcharge(amount, currency);

        public static string CardSurchargeLabel(string currency) =>
            (currency ?? "").Trim().ToUpperInvariant() switch
            {
                "CAD" => "Visa 2.4% · worldwide",
                "PEN" => "Visa 5% · Perú",
                _ => "Visa 2.4% · worldwide"
            };

        /// <summary>Culqi/Visa soles total: Yape price plus Perú 5% card surcharge, whole soles.</summary>
        public static decimal CulqiSoles(decimal usd, decimal listedPen = 0) =>
            Math.Round(YapeSoles(usd, listedPen) * (1m + PeruCardSurchargeRate), 0, MidpointRounding.AwayFromZero);

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
