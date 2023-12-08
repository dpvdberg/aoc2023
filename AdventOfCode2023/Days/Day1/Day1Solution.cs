using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day1;

public class Day1Solution : Solution
{
    public override int Day => 1;

    private readonly Dictionary<string, int> _numberName = new()
    {
        // {"zero", 0},
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 },
    };

    private string Replace(string input, bool first)
    {
        var containedKeys = _numberName.Keys
            .Where(input.Contains);
        var key = first ? containedKeys.MinBy(input.IndexOf) : containedKeys.MaxBy(input.IndexOf);
        return key != null ? input.ReplaceFirst(key, _numberName[key].ToString()) : input;
    }

    public override string Solve(SolutionPart part)
    {
        var lines = ReadInputLines();
        switch (part)
        {
            case SolutionPart.PartA:
                return lines.Select(l => l.ToCharArray())
                    .Select(a => a.Where(char.IsDigit).ToList())
                    .Select(a => int.Parse($"{a.First()}{a.Last()}"))
                    .Sum()
                    .ToString();
            case SolutionPart.PartB:
                lines = lines
                    .Select(l => Replace(l, true))
                    .Select(l => Replace(l, false))
                    .ToList();
                // Re-use logic of part A
                goto case SolutionPart.PartA;
            default:
                throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
    }
}