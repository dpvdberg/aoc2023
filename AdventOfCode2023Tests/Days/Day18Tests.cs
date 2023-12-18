using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day18;

namespace AdventOfCode2023Tests.Days;

public class Day18Tests
{
    
    private const string TestData = 
        """
        R 6 (#70c710)
        D 5 (#0dc571)
        L 2 (#5713f0)
        D 2 (#d2c081)
        R 2 (#59c680)
        D 2 (#411b91)
        L 5 (#8ceee2)
        U 2 (#caa173)
        L 1 (#1b58a2)
        U 2 (#caa171)
        R 2 (#7807d2)
        U 3 (#a77fa3)
        L 2 (#015232)
        U 2 (#7a21e3)
        """;
    
    [TestCase(TestData, ExpectedResult = "62")]
    public string PartA(string input)
    {
        return new Day18Solution(input).Solve(SolutionPart.PartA);
    }

    [TestCase(TestData, ExpectedResult = "952408144115")]
    public string PartB(string input)
    {
        return new Day18Solution(input).Solve(SolutionPart.PartB);
    }
}