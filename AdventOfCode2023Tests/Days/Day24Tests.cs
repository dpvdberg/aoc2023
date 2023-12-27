using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day23;
using AdventOfCode2023.Days.Day24;

namespace AdventOfCode2023Tests.Days;

public class Day24Tests
{
    private const string TestData =
        """
        19, 13, 30 @ -2,  1, -2
        18, 19, 22 @ -1, -1, -2
        20, 25, 34 @ -2, -2, -4
        12, 31, 28 @ -1, -2, -1
        20, 19, 15 @  1, -5, -3
        """;

    [TestCase(TestData, ExpectedResult = "2")]
    public string PartA(string input)
    {
        return new Day24Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(TestData, ExpectedResult = "47")]
    public string PartB(string input)
    {
        return new Day24Solution(input).Solve(SolutionPart.PartB);
    }
}