using System.Text.RegularExpressions;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day2;

public class Day2Solution : Solution
{
    public override int Day => 2;

    private List<Game> ParseGames()
    {
        var lineRegex = new Regex(@"Game (\d+):(.*)");

        return ReadInputLines()
            .Select(l => lineRegex.Match(l))
            .Select(m =>
                new Game(
                    int.Parse(m.Groups[1].Value),
                    m.Groups[2].Value.Split(';').Select(Hand.Parse).ToList()
                )
            ).ToList();
    }

    private int SumPossibleGameIds(Hand maxHand)
    {
        return ParseGames()
            .Where(g => g.Hands.All(h => h.IsCompliantWith(maxHand)))
            .Select(g => g.Id)
            .Sum();
    }

    private int SumPowerOfGames()
    {
        return ParseGames()
            .Select(g => g.Hands.Aggregate(Hand.Zero(), (agg, h) => agg.CombineMaxHand(h)))
            .Select(h => h.Power)
            .Sum();
    }

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => SumPossibleGameIds(new Hand(new Dictionary<CubeColor, int>
                {
                    { CubeColor.Red, 12 }, { CubeColor.Green, 13 }, { CubeColor.Blue, 14 },
                }))
                .ToString(),
            SolutionPart.PartB => SumPowerOfGames().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }


    private class Game
    {
        public Game(int id, List<Hand> hands)
        {
            Id = id;
            Hands = hands;
        }

        public int Id { get; }
        public List<Hand> Hands { get; }
    }

    private class Hand
    {
        public Hand(Dictionary<CubeColor, int> cubeCount)
        {
            CubeCount = cubeCount;
        }

        private Dictionary<CubeColor, int> CubeCount { get; }

        public Hand CombineMaxHand(Hand other)
        {
            return new Hand(
                Enum.GetValues(typeof(CubeColor))
                    .Cast<CubeColor>()
                    .ToDictionary(c => c, c =>
                        Math.Max(CubeCount.GetValueOrDefault(c, 0), other.CubeCount.GetValueOrDefault(c, 0))
                    )
            );
        }

        public static Hand Zero() => new Hand(
            Enum.GetValues(typeof(CubeColor))
                .Cast<CubeColor>()
                .ToDictionary(c => c, c => 0)
        );

        public static Hand Parse(string handString)
        {
            return new Hand(
                handString.Split(',')
                    .Select(s => s.Trim())
                    .Select(s => s.Split(' '))
                    .ToDictionary(
                        s => (CubeColor)Enum.Parse(typeof(CubeColor), s[1], true),
                        s => int.Parse(s[0]
                        ))
            );
        }

        public int Power =>
            Enum.GetValues(typeof(CubeColor))
                .Cast<CubeColor>()
                .Aggregate(1, (p, c) => p * CubeCount[c]);

        public bool IsCompliantWith(Hand other) =>
            Enum.GetValues(typeof(CubeColor))
                .Cast<CubeColor>()
                .All(c => IsCompliantWith(other, c));

        private bool IsCompliantWith(Hand other, CubeColor color) =>
            CubeCount.GetValueOrDefault(color, 0) <= other.CubeCount.GetValueOrDefault(color, 0);
    }

    private enum CubeColor
    {
        Blue,
        Red,
        Green
    }
}