namespace TheBestBean.Models
{
    public sealed class DualPriceModel
    {
        public decimal Usd { get; init; }
        public decimal Pen { get; init; }
        public bool Compact { get; init; }
        public bool From { get; init; }
        /// <summary>When set, shown instead of the Yape savings line (e.g. workshop booking).</summary>
        public string? FeeNote { get; init; }
    }
}
