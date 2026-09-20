using System.Text.RegularExpressions;

namespace TheBestBean.Services
{
    /// <summary>
    /// Storefront coffee titles: always <c>Variety [Cusco|Cajamarca]</c>.
    /// </summary>
    public static class CoffeeDisplayName
    {
        private static readonly Regex FarmerBracket = new(
            @"\s*\[\s*(?:Miguel\s+Cortez|Fransico\s+Rodriguez|Francisco\s+Rodriguez|Angel\s*M\.?|Ángel\s+Antonio[^\]]*|Luz\s+Marita[^\]]*)\s*\]\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly Regex AnyBracket = new(
            @"\s*\[([^\]]+)\]\s*$",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly string[] LotVarietyPrefixes =
        {
            "Geisha Korea",
            "Geisha R17",
            "Geisha Alto",
            "Inca Geisha",
        };

        private static readonly string[] BaseVarieties =
        {
            "Marsellesa",
            "Pachamara",
            "Bourbon",
            "Geisha",
            "Catuai",
            "Caturra",
            "Typica",
            "SL28",
            "SL09",
        };

        /// <summary>
        /// Canonical shop title from name / variety / region fields.
        /// </summary>
        public static string Standard(string? name, string? variety = null, string? regionOrLocation = null)
        {
            var cleanedName = CleanRawName(name);
            var region = ResolveRegion(regionOrLocation) ?? ResolveRegion(cleanedName);
            var vari = ResolveVariety(cleanedName, variety);

            if (string.IsNullOrWhiteSpace(vari))
            {
                return cleanedName;
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                return vari;
            }

            return $"{vari} [{region}]";
        }

        /// <summary>
        /// Display helper when only the stored title is available (already-normalized names pass through).
        /// </summary>
        public static string ForLab(string? name) => Standard(name);

        public static string? ResolveRegion(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (text.Contains("Cajamarca", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Jaén", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Jaen", StringComparison.OrdinalIgnoreCase))
            {
                return "Cajamarca";
            }

            if (text.Contains("Cusco", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Convención", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Convencion", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Quillabamba", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Inkawasi", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Echarate", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Vilcabamba", StringComparison.OrdinalIgnoreCase))
            {
                return "Cusco";
            }

            return null;
        }

        public static string ResolveVariety(string? name, string? varietyField = null)
        {
            var fromField = NormalizeVarietyToken(varietyField);
            if (!string.IsNullOrWhiteSpace(fromField) && !fromField.Contains('/'))
            {
                // Prefer explicit lot labels from the title when present (Geisha Korea, etc.).
                var lotFromName = MatchLotVariety(name);
                if (lotFromName != null)
                {
                    return lotFromName;
                }

                if (fromField.Equals("SL09", StringComparison.OrdinalIgnoreCase)
                    || fromField.StartsWith("SL09", StringComparison.OrdinalIgnoreCase))
                {
                    return "SL09";
                }

                return CanonicalVariety(fromField);
            }

            var lot = MatchLotVariety(name);
            if (lot != null)
            {
                return lot;
            }

            if (!string.IsNullOrWhiteSpace(fromField))
            {
                // e.g. "SL09 / Inca Geisha"
                if (fromField.Contains("SL09", StringComparison.OrdinalIgnoreCase))
                {
                    return "SL09";
                }

                return CanonicalVariety(fromField.Split('/')[0].Trim());
            }

            var cleaned = CleanRawName(name);
            if (cleaned.Contains("Super Highland", StringComparison.OrdinalIgnoreCase)
                || cleaned.Contains("SL09", StringComparison.OrdinalIgnoreCase))
            {
                return "SL09";
            }

            if (cleaned.Contains("SL28", StringComparison.OrdinalIgnoreCase))
            {
                return "SL28";
            }

            foreach (var v in BaseVarieties)
            {
                if (cleaned.Contains(v, StringComparison.OrdinalIgnoreCase))
                {
                    return CanonicalVariety(v);
                }
            }

            // "Region [Variety]" legacy: Cajamarca [Bourbon]
            var bracket = AnyBracket.Match(cleaned);
            if (bracket.Success)
            {
                var inner = CanonicalVariety(bracket.Groups[1].Value.Trim());
                if (BaseVarieties.Any(v => v.Equals(inner, StringComparison.OrdinalIgnoreCase)))
                {
                    return inner;
                }
            }

            return cleaned;
        }

        private static string? MatchLotVariety(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            foreach (var lot in LotVarietyPrefixes)
            {
                if (name.Contains(lot, StringComparison.OrdinalIgnoreCase))
                {
                    return lot;
                }
            }

            return null;
        }

        private static string CanonicalVariety(string token)
        {
            var t = NormalizeVarietyToken(token) ?? token.Trim();
            if (t.Equals("Meselessa", StringComparison.OrdinalIgnoreCase)
                || t.Equals("Mariseisa", StringComparison.OrdinalIgnoreCase)
                || t.Equals("Marseillaise", StringComparison.OrdinalIgnoreCase))
            {
                return "Marsellesa";
            }

            if (t.Equals("Caturai", StringComparison.OrdinalIgnoreCase)
                || t.Equals("Catuar", StringComparison.OrdinalIgnoreCase))
            {
                return "Catuai";
            }

            foreach (var v in BaseVarieties)
            {
                if (v.Equals(t, StringComparison.OrdinalIgnoreCase))
                {
                    return v;
                }
            }

            foreach (var lot in LotVarietyPrefixes)
            {
                if (lot.Equals(t, StringComparison.OrdinalIgnoreCase))
                {
                    return lot;
                }
            }

            return t;
        }

        private static string? NormalizeVarietyToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var t = value.Trim();
            t = Regex.Replace(t, @"Caturai", "Catuai", RegexOptions.IgnoreCase);
            t = Regex.Replace(t, @"Catuar", "Catuai", RegexOptions.IgnoreCase);
            return t;
        }

        private static string CleanRawName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }

            var cleaned = name.Trim();
            cleaned = FarmerBracket.Replace(cleaned, "");
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " ").Trim();
            cleaned = Regex.Replace(cleaned, @"Caturai", "Catuai", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"Catuar", "Catuai", RegexOptions.IgnoreCase);
            return cleaned;
        }
    }
}
