namespace TheBestBean.Services
{
    public record ExperienceAddOn(int Id, string Sku, string Name, string Description, decimal Usd);

    public static class ExperienceAddOns
    {
        public const string ProductType = "Extra";

        public static readonly ExperienceAddOn[] All =
        {
            new(91001, "water", "Mineral sparkling water", "Cold agua con gas for the cupping table.", 3m),
            new(91006, "bananas", "Bananas", "Fresh bananas during the session.", 2m),
            new(91002, "pretzels", "Pretzels", "Salty pretzel snack during the session.", 4m),
            new(91003, "peanuts", "Peanuts", "Roasted peanuts.", 3m),
            new(91004, "trail", "Trail mix", "Nuts and dried fruit.", 5m),
            new(91005, "avocado", "Avocado toast", "Made in the lab kitchen before or after the class.", 8m)
        };

        public static ExperienceAddOn? FindBySku(string? sku) =>
            string.IsNullOrWhiteSpace(sku)
                ? null
                : All.FirstOrDefault(a => a.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase));
    }
}
