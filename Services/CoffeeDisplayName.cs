using System.Text.RegularExpressions;

namespace TheBestBean.Services
{
    /// <summary>
    /// Storefront display helpers for coffee lot titles.
    /// </summary>
    public static class CoffeeDisplayName
    {
        // Trailing [Farmer / person] brackets — keep variety brackets like [Geisha], [Bourbon].
        private static readonly Regex FarmerBracket = new(
            @"\s*\[\s*(?:Miguel\s+Cortez|Fransico\s+Rodriguez|Francisco\s+Rodriguez|Angel\s*M\.?|Ángel\s+Antonio[^\]]*|Luz\s+Marita[^\]]*)\s*\]\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex GenericPersonBracket = new(
            @"\s*\[\s*[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ.]+){0,3}\s*\]\s*$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly HashSet<string> KeepBracketTokens = new(StringComparer.OrdinalIgnoreCase)
        {
            "Geisha", "Bourbon", "Marsellesa", "Catuai", "Catuar", "Caturra", "Typica",
            "SL09", "SL28", "Pachamara", "Inca Geisha", "SL09 / Inca Geisha"
        };

        public static string ForLab(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }

            var cleaned = name.Trim();
            cleaned = FarmerBracket.Replace(cleaned, "");
            cleaned = StripPersonBracketIfNotVariety(cleaned);
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " ").Trim();
            cleaned = Regex.Replace(cleaned, @"Caturai", "Catuar", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"Catuai", "Catuar", RegexOptions.IgnoreCase);
            return cleaned;
        }

        private static string StripPersonBracketIfNotVariety(string name)
        {
            var match = GenericPersonBracket.Match(name);
            if (!match.Success)
            {
                return name;
            }

            var inner = match.Value.Trim().Trim('[', ']').Trim();
            if (KeepBracketTokens.Contains(inner))
            {
                return name;
            }

            // Variety-only tokens are short / known; multi-word person names get stripped.
            if (inner.Contains(' ') || inner.EndsWith(".", StringComparison.Ordinal))
            {
                return name[..match.Index].TrimEnd();
            }

            return name;
        }
    }
}
