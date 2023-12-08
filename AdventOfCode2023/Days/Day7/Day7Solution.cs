using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day7;

public class Day7Solution : Solution
{
    public Day7Solution()
    {
    }

    public Day7Solution(string input) : base(input)
    {
    }

    private List<Hand> Parse()
        => ReadInputLines()
            .Select(l => l.Split(' '))
            .Select(s => new Hand(s[0], int.Parse(s[1])))
            .ToList();

    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    public override int Day => 7;

    private static bool _interpretJokers;

    public override string Solve(SolutionPart part)
    {
        _interpretJokers = part == SolutionPart.PartB;
        
        var hands = Parse();
        hands.Sort();
        
        return hands.Select((h, i) => h.Bid * (i + 1)).Sum().ToString();
    }

    private class Hand : IComparable<Hand>
    {
        public int Bid { get; }
        private List<char> Cards { get; }

        public Hand(string cards, int bid)
        {
            Bid = bid;
            Cards = cards.ToCharArray().ToList();
        }

        private HandType GetHandType()
        {
            return !_interpretJokers
                ? GetClassicHandType(Cards)
                : _orderedCharMap.Select(c => string.Join("", Cards).Replace('J', c).ToCharArray().ToList())
                    .Select(GetClassicHandType)
                    .Max();
        }

        private static HandType GetClassicHandType(List<char> cards)
        {
            return cards.Distinct().Count() switch
            {
                5 => HandType.HighCard,
                4 => HandType.OnePair,
                3 => cards.GroupBy(c => c).Select(g => g.Count()).Max() == 3
                    ? HandType.ThreeOfAKind
                    : HandType.TwoPair,
                2 => cards.GroupBy(c => c).Select(g => g.Count()).Max() == 4
                    ? HandType.FourOfAKind
                    : HandType.FullHouse,
                1 => HandType.FiveOfAKind,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private readonly List<char> _orderedCharMap = new List<char>()
            { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

        private readonly List<char> _orderedCharMapJoker = new List<char>()
            { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };

        public int CompareTo(Hand? other)
        {
            if (this.GetHandType() > other.GetHandType())
            {
                return 1;
            }

            if (this.GetHandType() < other.GetHandType())
            {
                return -1;
            }

            // Same hand, now lexicographically compare
            var map = _interpretJokers ? _orderedCharMapJoker : _orderedCharMap;
            
            return Cards.Zip(other.Cards, (m, o) =>
                map.IndexOf(m).CompareTo(map.IndexOf(o))
            ).FirstOrDefault(c => c != 0, 0);
        }
    }
}