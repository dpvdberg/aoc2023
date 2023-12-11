﻿using System.Numerics;
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
    
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ? new[] { Array.Empty<T>() } :
            elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => new[] {e}.Concat(c)));
    }

    public static T ParseEnum<T>(this char value)
    {
        return (T)Enum.ToObject(typeof(T), value);
    }

    private static readonly Regex DigitRegex = new(@"-?\d+", RegexOptions.Compiled);

    public static T LeastCommonMultiple<T>(this IEnumerable<T> values) where T : INumber<T>
        => values.Aggregate(MathUtils.LeastCommonMultiple);

    public static List<T> GetNumbers<T>(this string text)
        => DigitRegex.Matches(text)
            .SelectMany(m => m.Groups.Values)
            .Select(g => g.Value)
            .Select(v => (T)Convert.ChangeType(v, typeof(T))).ToList();
}