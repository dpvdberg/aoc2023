using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day20;

namespace AdventOfCode2023Tests.Days;

public class Day20Tests
{
    private const string TestDataSimple = 
        """
        broadcaster -> a, b, c
        %a -> b
        %b -> c
        %c -> inv
        &inv -> a
        """;
    
    private const string TestDataComplex = 
        """
        broadcaster -> a
        %a -> inv, con
        &inv -> b
        %b -> con
        &con -> output
        """;
    
    [TestCase(TestDataSimple, ExpectedResult = "32000000")]
    public string PartA_Simple(string input)
    {
        return new Day20Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(TestDataComplex, ExpectedResult = "11687500")]
    public string PartA_Complex(string input)
    {
        return new Day20Solution(input).Solve(SolutionPart.PartA);
    }
}