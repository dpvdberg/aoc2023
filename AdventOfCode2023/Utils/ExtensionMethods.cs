using System.Text.RegularExpressions;

namespace AdventOfCode2023.Utils;

public static class ExtensionMethods
{
    public static string ReplaceFirst(this string text, string search, string replace)
    {
        int pos = text.IndexOf(search, StringComparison.Ordinal);
        if (pos < 0)
        {
            return text;
        }

        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    private static readonly Regex DigitRegex = new(@"\d+", RegexOptions.Compiled);

    public static List<T> GetNumbers<T>(this string text)
        => DigitRegex.Matches(text)
            .SelectMany(m => m.Groups.Values)
            .Select(g => g.Value)
            .Select(v => (T)Convert.ChangeType(v, typeof(T))).ToList();
}