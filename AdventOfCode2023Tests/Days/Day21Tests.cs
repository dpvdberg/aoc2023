using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day21;

namespace AdventOfCode2023Tests.Days;

public class Day21Tests
{
    private const string TestData = 
        """
        ...........
        .....###.#.
        .###.##..#.
        ..#.#...#..
        ....#.#....
        .##..S####.
        .##..#...#.
        .......##..
        .##.#.####.
        .##..##.##.
        ...........
        """;
    
    [TestCase(TestData, ExpectedResult = "16")]
    public string PartA_Naive(string input)
    {
        var solution = new Day21SolutionNaive(input, false);
        return solution.Parse(false).Walk(6).ToString();
    }
    
    [TestCase(ExpectedResult = "3830")]
    public string PartA_RealData()
    {
        return new Day21SolutionNaive(false).Solve(SolutionPart.PartA);
    }
}