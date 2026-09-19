using System.Globalization;
using TheBestBean.Models;

namespace TheBestBean.Services;

/// <summary>Meeting point copy and map pin for Cusco workshop hosts.</summary>
public static class ExperienceVenue
{
    public sealed record Venue(
        string AreaRegion,
        string SpotName,
        string? WalkNote,
        double Latitude,
        double Longitude);

    public static Venue? Resolve(Experience? experience)
    {
        if (experience == null)
        {
            return null;
        }

        var title = experience.Title ?? string.Empty;

        if (title.Contains("Odar", StringComparison.OrdinalIgnoreCase))
        {
            // Misky factory: Av. República de Bolivia C-11, Parque Industrial, Wanchaq.
            return new Venue("Cusco City", "República de Bolivia C-11", "Parque Industrial, Wanchaq", -13.53205, -71.94598);
        }

        if ((title.Contains("Coffee Laboratory", StringComparison.OrdinalIgnoreCase)
             || title.Contains("Coffee Lab", StringComparison.OrdinalIgnoreCase))
            && !title.Contains("Odar", StringComparison.OrdinalIgnoreCase)
            && !title.Contains("Cinthya", StringComparison.OrdinalIgnoreCase)
            && !title.Contains("Cynthia", StringComparison.OrdinalIgnoreCase))
        {
            // Google listing: Belen Coffee Lab, Santiago 08484 (not San Francisco / centro).
            return new Venue("Cusco City", "Santiago", "Belén · 15 min from Plaza de Armas", -13.52825, -71.98236);
        }

        // Calle Santa Teresa 365 — walk note points to the SUNAT in front of the door.
        if (title.Contains("Tasting Hour", StringComparison.OrdinalIgnoreCase)
            || title.Contains("Brew Your Own", StringComparison.OrdinalIgnoreCase)
            || title.Contains("Introduction to Cupping", StringComparison.OrdinalIgnoreCase)
            || title.Contains("Cinthya", StringComparison.OrdinalIgnoreCase)
            || title.Contains("Cynthia", StringComparison.OrdinalIgnoreCase)
            || title.Contains("Peru Tasting", StringComparison.OrdinalIgnoreCase))
        {
            return new Venue("Cusco City", "Calle Santa Teresa 365", "in front of the SUNAT", -13.51612, -71.98120);
        }

        var category = experience.Category ?? string.Empty;
        var location = experience.Location ?? string.Empty;
        if (category.Contains("Urban", StringComparison.OrdinalIgnoreCase)
            && location.Contains("Cusco", StringComparison.OrdinalIgnoreCase))
        {
            return new Venue("Cusco City", "Plaza de Armas", "5 min walk", -13.5164, -71.9781);
        }

        return null;
    }

    public static string InlineLabel(Venue venue) => $"{venue.AreaRegion} · {venue.SpotName}";

    public static string MapEmbedUrl(Venue venue) =>
        string.Create(CultureInfo.InvariantCulture,
            $"https://www.google.com/maps?q=loc:{venue.Latitude},{venue.Longitude}&hl=en&z=18&output=embed");

    public static string MapOpenUrl(Venue venue) =>
        string.Create(CultureInfo.InvariantCulture,
            $"https://www.google.com/maps/place/{venue.Latitude},{venue.Longitude}/@{venue.Latitude},{venue.Longitude},19z");
}
