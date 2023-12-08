using System.Text.RegularExpressions;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day4;

public class Day4Solution : Solution
{
    public Day4Solution()
    {
    }

    public Day4Solution(string input) : base(input)
    {
    }

    private List<ScratchCard> ParseCards()
        => ReadInputLines().Select(ScratchCard.ParseFromLine).ToList();


    public override int Day => 4;

    public override string Solve(SolutionPart part)
    {
        var cards = ParseCards();
        return part switch
        {
            SolutionPart.PartA => cards.Select(c => c.CountPower).Sum().ToString(),
            SolutionPart.PartB => cards.Sum(c => c.GetDescendants(cards).Count).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private class ScratchCard
    {
        public int Id { get; }
        public HashSet<int> Valid { get; }
        public HashSet<int> Provided { get; }

        private List<ScratchCard>? _descendantCache = null;

        public List<ScratchCard> GetDescendants(List<ScratchCard> cards)
        {
            if (_descendantCache != null)
            {
                return _descendantCache;
            }

            var descendants = Enumerable.Range(Id, CountWinning)
                .Where(i => i < cards.Count)
                .Select(i => cards[i])
                .SelectMany(c => c.GetDescendants(cards)).ToList();
            
            descendants.Add(this);
            
            _descendantCache = descendants;
            
            return descendants;
        }

        public int CountPower => (int) Math.Pow(2, CountWinning - 1);

        public int CountWinning => Provided.Intersect(Valid).Count();

        private static readonly Regex ParseRegex = new(@"Card\s+(\d+): ([\d+\s*]+)\|([\s*\d+]+)");
        private static readonly Regex DigitRegex = new(@"\d+");

        public static ScratchCard ParseFromLine(string line)
        {
            var match = ParseRegex.Match(line);
            return new ScratchCard(
                int.Parse(match.Groups[1].Value),
                DigitRegex.Matches(match.Groups[2].Value)
                    .SelectMany(m => m.Groups.Values)
                    .Select(g => g.Value)
                    .Select(int.Parse).ToList(),
                DigitRegex.Matches(match.Groups[3].Value)
                    .SelectMany(m => m.Groups.Values)
                    .Select(g => g.Value)
                    .Select(int.Parse).ToList()
            );
        }

        public ScratchCard(int id, List<int> valid, List<int> provided)
        {
            Id = id;
            Valid = new HashSet<int>(valid);
            Provided = new HashSet<int>(provided);
        }
    }
}