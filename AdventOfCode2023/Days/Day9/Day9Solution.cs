using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day9;

public class Day9Solution : Solution
{
    public override int Day => 9;

    public Day9Solution()
    {
    }

    public Day9Solution(string input) : base(input)
    {
    }

    private List<NumberSequence> Parse()
        => ReadInputLines().Select(l => l.GetNumbers<int>()).Select(n => new NumberSequence(n)).ToList();

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse()
                .Select(l => l.Extrapolate())
                .Select(l => l.Numbers.Last())
                .Sum().ToString(),
            SolutionPart.PartB => Parse()
                .Select(l => l.Extrapolate())
                .Select(l => l.Numbers.First())
                .Sum().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private class NumberSequence
    {
        public NumberSequence(List<int> numbers)
        {
            Numbers = numbers;
        }

        public List<int> Numbers { get; }

        private NumberSequence GetDifferenceSequence()
            => new (Numbers.Zip(Numbers.Skip(1), (a, b) => b - a).ToList());

        private bool IsZero => Numbers.All(n => n == 0);
        
        public NumberSequence Extrapolate()
        {
            var n = new List<int>(Numbers);
            if (IsZero)
            {
                n.Add(0);
                n.Add(0);
                return new NumberSequence(n);
            }
            
            // Get extrapolated difference sequence
            var diffExt = GetDifferenceSequence().Extrapolate();
            
            // Add to start
            n.Insert(0, Numbers.First() - diffExt.Numbers.First());
            
            // Add to end
            n.Add(Numbers.Last() + diffExt.Numbers.Last());

            return new NumberSequence(n);
        }
    }
}