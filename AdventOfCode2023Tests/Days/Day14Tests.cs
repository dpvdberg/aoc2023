using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day14;

namespace AdventOfCode2023Tests.Days;

public class Day14Tests
{
    private const string TestData =
        """
        O....#....
        O.OO#....#
        .....##...
        OO.#O....O
        .O.....O#.
        O.#..O.#.#
        ..O..#O..O
        .......O..
        #....###..
        #OO..#....
        """;
    
    [TestCase(TestData, ExpectedResult = "136")]
    public string PartA(string input)
    {
        return new Day14Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(TestData, ExpectedResult = "64")]
    public string PartB(string input)
    {
        return new Day14Solution(input).Solve(SolutionPart.PartB);
    }
}