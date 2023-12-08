using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day7;

namespace AdventOfCode2023Tests.Days.Day3;

public class Day7Tests
{
    private const string testData =
        """
        32T3K 765
        T55J5 684
        KK677 28
        KTJJT 220
        QQQJA 483
        """;
    
    [TestCase(testData, ExpectedResult = "6440")]
    public string PartA(string input)
    {
        return new Day7Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(testData, ExpectedResult = "5905")]
    public string PartB(string input)
    {
        return new Day7Solution(input).Solve(SolutionPart.PartB);
    }
}