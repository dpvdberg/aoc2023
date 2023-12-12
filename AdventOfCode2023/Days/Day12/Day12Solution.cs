using System.Text.RegularExpressions;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day12;

public class Day12Solution : Solution
{
    public Day12Solution()
    {
    }

    private List<SpringArrangement> Parse(SolutionPart part)
    {
        var arrangements = ReadInputLines().Select(l => l.Split(' '))
            .Select(s => (s[0], s[1]))
            .Select(s => new SpringArrangement(
                s.Item1.ToCharArray().Select(c => c.ParseEnum<SpringType>()).ToList(),
                s.Item2.Split(',').Select(c => new SpringGroup(int.Parse(c), 0)).ToList()
            )).ToList();

        if (part == SolutionPart.PartA)
        {
            return arrangements;
        }

        foreach (var a in arrangements)
        {
            a.Groups = a.Groups.Repeat(4).ToList();
            a.Springs.Add(SpringType.Unknown);
            a.Springs = a.Springs.Repeat(4).ToList();
            a.Springs.RemoveAt(a.Springs.Count - 1);
        }

        return arrangements;
    }

    public Day12Solution(string input) : base(input)
    {
    }

    public override int Day => 12;

    public override string Solve(SolutionPart part)
        => Parse(part).Sum(a => a.GetPossibleArrangements()).ToString();

    private class SpringArrangement
    {
        public SpringArrangement(List<SpringType> springs, List<SpringGroup> groups)
        {
            Springs = springs;
            Groups = groups;
        }

        public List<SpringType> Springs { get; set; }

        public List<SpringGroup> Groups { get; set; }

        private static readonly Dictionary<string, long> Cache = new();

        public long GetPossibleArrangements()
        {
            // Caching make this much faster...
            if (!Cache.ContainsKey(ToString()))
            {
                Cache[ToString()] = CountPossibleArrangements();
            }

            return Cache[ToString()];
        }

        private long CountPossibleArrangements()
        {
            return Springs.FirstOrDefault() switch
            {
                SpringType.Operational =>
                    new SpringArrangement(Springs.Skip(1).ToList(), Groups).GetPossibleArrangements(),
                SpringType.Unknown => ProcessUnknown(),
                SpringType.Damaged => ProcessDamaged(),
                _ => Groups.Any() ? 0 : 1,
            };
        }


        private long ProcessUnknown()
            =>
                // Divide and Conquer
                new SpringArrangement(
                    new List<SpringType> { SpringType.Operational }.Concat(Springs.Skip(1)).ToList(),
                    Groups
                ).GetPossibleArrangements()
                +
                new SpringArrangement(
                    new List<SpringType> { SpringType.Damaged }.Concat(Springs.Skip(1)).ToList(),
                    Groups
                ).GetPossibleArrangements();

        private long ProcessDamaged()
        {
            if (!Groups.Any())
            {
                return 0;
            }

            var group = Groups.First();

            var potentialGroupSize = Springs.TakeWhile(s => s is SpringType.Damaged or SpringType.Unknown).Count();

            if (potentialGroupSize < group.Size)
            {
                // Group too small, doesn't fit
                return 0;
            }

            if (Springs.Count == group.Size)
            {
                // This group fits exactly
                return Groups.Count == 1 ? 1 : 0;
            }

            if (Springs[group.Size] == SpringType.Damaged)
            {
                // Group too large, doesn't fit
                return 0;
            }

            // Apply the group and continue
            return new SpringArrangement(
                Springs.Skip(group.Size + 1).ToList(),
                Groups.Skip(1).ToList()
            ).GetPossibleArrangements();
        }

        public override string ToString()
        {
            return
                $"{string.Join("", Springs.Select(s => (char)s).ToList())}" +
                $" [{string.Join(",", Groups.Select(g => g.Size))}]";
        }
    }

    private class SpringGroup
    {
        public SpringGroup(int size, int index)
        {
            Size = size;
            Index = index;
        }

        public int Size { get; }
        public int Index { get; }
    }

    public enum SpringType
    {
        Unknown = '?',
        Operational = '.',
        Damaged = '#'
    }
}