using System.Text.RegularExpressions;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    public static class ExperienceGeo
    {
        public readonly record struct Place(string Region, string PlaceKey, string RegionLabel, string PlaceLabel);


        /// <summary>Display durations as 1hr / 2.5hrs instead of 1 HOUR / 2.5 HOURS.</summary>
        public static string FormatDuration(string? duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return string.Empty;
            var s = duration.Trim();
            s = Regex.Replace(s, @"\bHOURS\b", "hrs", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bHOUR\b", "hr", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"(\d+(?:\.\d+)?)\s*(hrs?)\b", "$1$2", RegexOptions.IgnoreCase);
            return s;
        }


        public static Place Locate(Experience e)
        {
            var blob = $"{e.Location} {e.Title} {e.Tag}".ToLowerInvariant();

            if (blob.Contains("jaén") || blob.Contains("jaen") || blob.Contains("cajamarca") || blob.Contains("catarata"))
            {
                return new("cajamarca", "jaen", "Cajamarca", "Jaén");
            }

            if (blob.Contains("santa teresa") || blob.Contains("hilda"))
            {
                return new("cusco", "santa-teresa", "Cusco", "Santa Teresa");
            }

            if (blob.Contains("calca"))
            {
                return new("cusco", "calca", "Cusco", "Calca");
            }

            if (blob.Contains("lima") || blob.Contains("barranco"))
            {
                return new("lima", "barranco", "Lima", "Barranco");
            }

            return new("cusco", "city", "Cusco", "Cusco City");
        }

        public static bool IsMonWed(string? month)
        {
            var m = (month ?? "").Trim().ToLowerInvariant().Replace(" ", "");
            return m.Contains("mon-wed") || m.Contains("monwed");
        }

        public static string WhenKey(string? month)
        {
            var m = (month ?? "").Trim().ToLowerInvariant();
            if (m.StartsWith("aug")) return "august";
            if (m.StartsWith("sep")) return "september";
            if (m.StartsWith("oct")) return "october";
            if (m.Contains("year")) return "year-round";
            if (m.Contains("harvest")) return "harvest";
            return m;
        }

        public static string WhenLabel(string key) => key switch
        {
            "august" => "August",
            "september" => "September",
            "october" => "October",
            "year-round" => "Year-round",
            "harvest" => "Harvest",
            _ => key
        };
    }
}
