using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day15;

namespace AdventOfCode2023Tests.Days;

public class Day15Tests
{
    private const string TestData = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
    
    [TestCase(TestData, ExpectedResult = "1320")]
    public string PartA(string input)
    {
        return new Day15Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(TestData, ExpectedResult = "145")]
    public string PartB(string input)
    {
        return new Day15Solution(input).Solve(SolutionPart.PartB);
    }
}