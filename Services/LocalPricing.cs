namespace TheBestBean.Services
{
    /// <summary>
    /// Local Yape/Plin soles vs international card USD.
    /// Yape uses the listed PEN price (or 3.75× USD). Card is shown in USD
    /// and as a soles equivalent at a higher rate so bank FX + card fees are obvious.
    /// </summary>
    public static class LocalPricing
    {
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
    }
}
