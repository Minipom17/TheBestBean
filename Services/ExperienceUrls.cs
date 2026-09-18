using TheBestBean.Models;

namespace TheBestBean.Services;

public static class ExperienceUrls
{
    public static string PublicPath(Experience experience) =>
        PublicPath(experience.Id, experience.Category);

    public static string PublicPath(int id, string? category) =>
        string.Equals(category, "Expedition", StringComparison.OrdinalIgnoreCase)
            ? $"/Expedition/{id}"
            : $"/Workshop/{id}";
}
