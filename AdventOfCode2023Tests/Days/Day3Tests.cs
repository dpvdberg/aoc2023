using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day3;

namespace AdventOfCode2023Tests.Days;

public class Day3Tests
{
    private const string ExampleData = """
        467..114..
        ...*......
        ..35..633.
        ......#...
        617*......
        .....+.58.
        ..592.....
        ......755.
        ...$.*....
        .664.598..
        """;

    [TestCase(ExampleData, ExpectedResult = "4361")]
    public string PartA(string input)
    {
        return new Day3Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(ExampleData, ExpectedResult = "467835")]
    public string PartB(string input)
    {
        return new Day3Solution(input).Solve(SolutionPart.PartB);
    }
}