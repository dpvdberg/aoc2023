using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day11;

namespace AdventOfCode2023Tests.Days.Day3;

public class Day11Tests
{
    
    
    [TestCase(
        """
        ...#......
        .......#..
        #.........
        ..........
        ......#...
        .#........
        .........#
        ..........
        .......#..
        #...#.....
        """, ExpectedResult = "374")]
    public string PartA(string input)
    {
        return new Day11Solution(input).Solve(SolutionPart.PartA);
    }
}