using System;
using System.Linq;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public static class CoffeeCardTitle
    {
        private static readonly string[] Places =
        {
            "cajamarca", "cusco", "jaén", "jaen", "convención", "convencion",
            "peru", "colombia", "santa teresa", "quillabamba"
        };

        public readonly record struct Parts(string Variety, string? Location, string? Farmer);

        public static Parts From(CoffeeBean bean)
        {
            var parsed = ParseName(bean.Name ?? "");
            var variety = FirstNonEmpty(parsed.Variety, Clean(bean.Variety));
            var location = FirstNonEmpty(ShortRegion(bean.CoffeeRegion?.Name), parsed.Location);
            var farmer = FirstNonEmpty(
                UsableFarmer(bean.Producer),
                parsed.Farmer);

            return new Parts(variety, location, farmer == null ? null : ShortFarmer(farmer));
        }

        public static string Line(CoffeeBean bean) => Line(From(bean));

        public static string Line(Parts parts)
        {
            if (string.IsNullOrWhiteSpace(parts.Variety))
            {
                return "";
            }

            return string.IsNullOrWhiteSpace(parts.Location)
                ? parts.Variety
                : $"{parts.Variety} [{parts.Location}]";
        }

        private static Parts ParseName(string name)
        {
            name = name.Trim();
            string? inner = null;
            var open = name.LastIndexOf('[');
            var close = name.LastIndexOf(']');
            if (open >= 0 && close > open)
            {
                inner = name[(open + 1)..close].Trim();
                name = name[..open].Trim();
            }

            string variety;
            string? location = null;
            string? farmer = null;

            var dash = name.LastIndexOf(" - ", StringComparison.Ordinal);
            if (dash > 0)
            {
                variety = name[..dash].Trim();
                var after = name[(dash + 3)..].Trim();
                if (IsPlace(after)) location = NormalizePlace(after);
                else variety = name;
            }
            else
            {
                variety = name;
            }

            if (!string.IsNullOrWhiteSpace(inner))
            {
                if (IsPlace(inner))
                {
                    location ??= NormalizePlace(inner);
                }
                else if (IsPlace(variety))
                {
                    location = NormalizePlace(variety);
                    variety = inner;
                }
                else if (LooksLikeFarmer(inner))
                {
                    farmer = inner;
                }
            }

            return new Parts(variety, location, farmer);
        }

        private static string? ShortRegion(string? region)
        {
            if (string.IsNullOrWhiteSpace(region)) return null;
            if (ContainsPlace(region, "cajamarca")) return "Cajamarca";
            if (ContainsPlace(region, "cusco") || ContainsPlace(region, "convención") || ContainsPlace(region, "convencion"))
                return "Cusco";
            if (ContainsPlace(region, "jaén") || ContainsPlace(region, "jaen")) return "Jaén";
            var cut = region.IndexOf('[');
            if (cut > 0) region = region[..cut].Trim();
            var comma = region.LastIndexOf(',');
            return comma > 0 ? region[(comma + 1)..].Trim() : region.Trim();
        }

        private static string? UsableFarmer(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var n = value.ToLowerInvariant();
            if (n.Contains("producer") || n.Contains("smallholder") || n.Contains("cooperative")
                || n.Contains("coop") || n.Contains("highland") || n.Equals("inkawasi"))
            {
                return null;
            }
            return value.Trim();
        }

        private static string ShortFarmer(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 2) return name;
            if (name.Contains(" y ", StringComparison.OrdinalIgnoreCase)
                || name.Contains(" and ", StringComparison.OrdinalIgnoreCase))
            {
                return $"{parts[0]} {parts[1]}";
            }
            return $"{parts[0]} {parts[^1]}";
        }

        private static bool LooksLikeFarmer(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || IsPlace(value)) return false;
            if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^(SL|R)\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                return false;
            return value.Contains(' ') || value.Contains('.');
        }

        private static bool IsPlace(string value) =>
            Places.Any(p => value.Contains(p, StringComparison.OrdinalIgnoreCase));

        private static bool ContainsPlace(string value, string place) =>
            value.Contains(place, StringComparison.OrdinalIgnoreCase);

        private static string NormalizePlace(string value)
        {
            if (ContainsPlace(value, "cajamarca")) return "Cajamarca";
            if (ContainsPlace(value, "cusco")) return "Cusco";
            if (ContainsPlace(value, "jaén") || ContainsPlace(value, "jaen")) return "Jaén";
            return value.Trim();
        }

        private static string Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "" : value.Trim();

        private static string FirstNonEmpty(params string?[] values) =>
            values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? "";
    }
}
