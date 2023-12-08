using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day6;

namespace AdventOfCode2023Tests.Days.Day3;

public class Day6Tests
{
    
    
    [TestCase(
        """
        Time:      7  15   30
        Distance:  9  40  200
        """, ExpectedResult = "288")]
    public string PartA(string input)
    {
        return new Day6Solution(input).Solve(SolutionPart.PartA);
    }
}