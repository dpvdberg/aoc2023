using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day22;

namespace AdventOfCode2023Tests.Days;

public class Day22Tests
{
    private const string TestData = 
        """
        1,0,1~1,2,1
        0,0,2~2,0,2
        0,2,3~2,2,3
        0,0,4~0,2,4
        2,0,5~2,2,5
        0,1,6~2,1,6
        1,1,8~1,1,9
        """;
    
    [TestCase(TestData, ExpectedResult = "5")]
    public string PartA(string input)
    {
        return new Day22Solution(input).Solve(SolutionPart.PartA);
    }
}