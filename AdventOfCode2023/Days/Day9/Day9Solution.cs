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
        var numberSequences = Parse();

        if (part == SolutionPart.PartB)
        {
            numberSequences.ForEach(n => n.Numbers.Reverse());
        }

        return numberSequences
            .Select(l => l.Extrapolate())
            .Sum().ToString();
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
        
        public int Extrapolate()
        {
            if (IsZero)
            {
                return 0;
            }
            
            return Numbers.Last() + GetDifferenceSequence().Extrapolate();
        }
    }
}