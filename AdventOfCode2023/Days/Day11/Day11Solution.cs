using System.Threading.Tasks.Dataflow;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day11;

public class Day11Solution : Solution
{
    public Day11Solution()
    {
    }

    public Day11Solution(string input) : base(input)
    {
    }

    private GalaxyImage Parse(int expansionFactor)
        => new(ReadInputLines().SelectMany((l, i) =>
            l.ToCharArray()
                .Select((c, j) => new KeyValuePair<int, int>(c, j))
                .Where(c => c.Key == '#')
                .Select(c => new Galaxy(c.Value, i))
        ).ToList(), expansionFactor);

    public override int Day => 11;

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse(2).DistanceSum().ToString(),
            SolutionPart.PartB => Parse(1000000).DistanceSum().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private class GalaxyImage
    {
        public GalaxyImage(List<Galaxy> galaxies, int expansionFactor)
        {
            Galaxies = galaxies;
            ExpansionFactor = expansionFactor;
        }

        private List<int> GetGalaxyPositions(Axis axis) =>
            Galaxies.Select(g => axis == Axis.X ? g.X : g.Y).ToList();

        private int Min(Axis axis) => GetGalaxyPositions(axis).Min();
        private int Max(Axis axis) => GetGalaxyPositions(axis).Max();

        private readonly Dictionary<Axis, List<int>> _expansionDict = new();

        private List<int> FindExpansions(Axis axis)
        {
            if (_expansionDict.TryGetValue(axis, out var findExpansions))
            {
                return findExpansions;
            }

            var expansions = Enumerable.Range(Min(axis), Max(axis) - Min(axis) + 1)
                .Where(a => !GetGalaxyPositions(axis).Contains(a)).ToList();
            _expansionDict[axis] = expansions;
            return expansions;
        }

        private List<int> FindExpansions(Axis axis, int a, int b)
            => FindExpansions(axis).Where(v => v >= a && v <= b).ToList();

        /**
         * Count expansions between two galaxies
         */
        public long CountExpansions(Galaxy a, Galaxy b)
            => FindExpansions(Axis.X, Math.Min(a.X, b.X), Math.Max(a.X, b.X)).Count
               + FindExpansions(Axis.Y, Math.Min(a.Y, b.Y), Math.Max(a.Y, b.Y)).Count;

        public long DistanceSum() =>
            Galaxies.Combinations(2).Select(c => c.ToList())
                .Select(c => (c[0], c[1]))
                .Sum(c => c.Item1.ManhattanDistance(c.Item2) + (ExpansionFactor - 1) * CountExpansions(c.Item1, c.Item2));

        private List<Galaxy> Galaxies { get; }
        private int ExpansionFactor { get; }
    }

    private enum Axis
    {
        X,
        Y
    }

    private class Galaxy
    {
        public Galaxy(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public int ManhattanDistance(Galaxy other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }
    }
}