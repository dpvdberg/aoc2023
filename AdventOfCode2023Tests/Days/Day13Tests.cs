using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day13;

namespace AdventOfCode2023Tests.Days;

public class Day13Tests
{
    private const string TestData =
        """
        #.##..##.
        ..#.##.#.
        ##......#
        ##......#
        ..#.##.#.
        ..##..##.
        #.#.##.#.

        #...##..#
        #....#..#
        ..##..###
        #####.##.
        #####.##.
        ..##..###
        #....#..#
        """;

    [TestCase(TestData, ExpectedResult = "405")]
    public string PartA(string input)
    {
        return new Day13Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(TestData, ExpectedResult = "400")]
    public string PartB(string input)
    {
        return new Day13Solution(input).Solve(SolutionPart.PartB);
    }
}