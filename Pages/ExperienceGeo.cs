using TheBestBean.Models;

namespace TheBestBean.Pages
{
    public static class ExperienceGeo
    {
        public readonly record struct Place(string Region, string PlaceKey, string RegionLabel, string PlaceLabel);

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

            return new("cusco", "city", "Cusco", "City");
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
