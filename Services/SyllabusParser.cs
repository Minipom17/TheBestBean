using System.Text.RegularExpressions;

namespace TheBestBean.Services;

public record SyllabusItem(string Label, string Title, string Time, string? DetailBody, IReadOnlyList<string> Tags);

public static class SyllabusParser
{
    private static readonly Regex TimeOnly = new(
        @"^\d+(?:\.\d+)?\s*(?:min(?:ute)?s?|hrs?|hours?)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex TrailingTime = new(
        @"^(.*)\((\d+(?:\.\d+)?\s*(?:min(?:ute)?s?|hrs?|hours?))\)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly char[] TagSplit = [',', '·'];

    public static SyllabusItem Parse(string item, int index)
    {
        var detailParts = item.Split(new[] { "||" }, StringSplitOptions.None);
        var mainPart = detailParts[0].Trim();
        var detailBody = detailParts.Length > 1 ? detailParts[1].Trim() : null;
        var tags = ParseTags(detailParts.Length > 2 ? string.Join(",", detailParts.Skip(2)) : null);

        if (detailBody != null && detailBody.Contains("||", StringComparison.Ordinal))
        {
            var leftover = detailBody.Split(new[] { "||" }, 2, StringSplitOptions.None);
            detailBody = leftover[0].Trim();
            tags = ParseTags(string.Join(",", tags.Concat(ParseTags(leftover[1]))));
        }

        var colonParts = mainPart.Split(':', 2);
        var label = colonParts.Length > 1 ? colonParts[0].Trim() : $"Part {index + 1}";
        var rest = colonParts.Length > 1 ? colonParts[1].Trim() : mainPart;

        if (label.Equals("Introduction", StringComparison.OrdinalIgnoreCase))
        {
            label = "Welcome";
        }

        string title;
        string time;

        if (rest.Contains('|'))
        {
            var pipeParts = rest.Split('|', 2);
            title = pipeParts[0].Trim();
            time = NormalizeTime(pipeParts[1].Trim());
        }
        else if (TimeOnly.IsMatch(rest))
        {
            title = "";
            time = NormalizeTime(rest);
        }
        else
        {
            var match = TrailingTime.Match(rest);
            if (match.Success)
            {
                title = match.Groups[1].Value.Trim().TrimEnd('—', '-', '–', ' ');
                time = NormalizeTime(match.Groups[2].Value.Trim());
            }
            else
            {
                title = rest;
                time = "";
            }
        }

        return new SyllabusItem(label, title, time, detailBody, tags);
    }

    private static IReadOnlyList<string> ParseTags(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        return raw.Split(TagSplit, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string NormalizeTime(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
        {
            return "";
        }

        var t = time.Trim();
        t = Regex.Replace(t, @"\bmins\b", "min", RegexOptions.IgnoreCase);
        t = Regex.Replace(t, @"\bhours\b", "hrs", RegexOptions.IgnoreCase);
        t = Regex.Replace(t, @"\bhour\b", "hr", RegexOptions.IgnoreCase);
        t = Regex.Replace(t, @"(\d+(?:\.\d+)?)\s*(hrs?)\b", "$1$2", RegexOptions.IgnoreCase);
        return t;
    }
}
