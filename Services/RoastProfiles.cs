namespace TheBestBean.Services
{
    public static class RoastProfiles
    {
        public const string Lab = "profile";
        public const string Light = "light";
        public const string Medium = "medium";
        public const string Dark = "dark";

        public static string Normalize(string? value) =>
            (value ?? Lab).Trim().ToLowerInvariant() switch
            {
                "light" or "city" => Light,
                "medium" or "full city" => Medium,
                "dark" or "french" => Dark,
                "profile" or "lab" or "our roast" or "recommended" => Lab,
                _ => string.IsNullOrWhiteSpace(value) ? Lab : value.Trim()
            };

        public static string Label(string? value)
        {
            var key = Normalize(value);
            return key switch
            {
                Light => "Light roast",
                Medium => "Medium roast",
                Dark => "Dark roast",
                Lab => "Our roast",
                _ => value!.Trim()
            };
        }

        public static string CartLine(string? value) =>
            Normalize(value) == Lab ? "Our roast (recommended)" : Label(value);

        public static bool IsCoffeeProduct(string? productType) =>
            string.Equals(productType, "Bean", StringComparison.OrdinalIgnoreCase)
            || string.Equals(productType, "CoffeeBean", StringComparison.OrdinalIgnoreCase);
    }
}
