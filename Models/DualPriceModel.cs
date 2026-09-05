namespace TheBestBean.Models
{
    public sealed class DualPriceModel
    {
        public decimal Usd { get; init; }
        public decimal Pen { get; init; }
        public bool Compact { get; init; }
        public bool From { get; init; }
    }
}
