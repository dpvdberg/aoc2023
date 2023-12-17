using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day16;

namespace AdventOfCode2023Tests.Days;

public class Day16Tests
{
    
    private const string TestData = 
        """
        .|...\....
        |.-.\.....
        .....|-...
        ........|.
        ..........
        .........\
        ..../.\\..
        .-.-/..|..
        .|....-|.\
        ..//.|....
        """;
    
    [TestCase(TestData, ExpectedResult = "46")]
    public string PartA(string input)
    {
        return new Day16Solution(input).Solve(SolutionPart.PartA);
    }
    
    
}