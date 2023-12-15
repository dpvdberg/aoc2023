using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day8;

namespace AdventOfCode2023Tests.Days;

public class Day8Tests
{
    private const string SampleA =
        """
        RL

        AAA = (BBB, CCC)
        BBB = (DDD, EEE)
        CCC = (ZZZ, GGG)
        DDD = (DDD, DDD)
        EEE = (EEE, EEE)
        GGG = (GGG, GGG)
        ZZZ = (ZZZ, ZZZ)
        """;
    
    private const string SampleB =
        """
        LLR

        AAA = (BBB, BBB)
        BBB = (AAA, ZZZ)
        ZZZ = (ZZZ, ZZZ)
        """;
    
    private const string SampleC =
        """
        LR

        11A = (11B, XXX)
        11B = (XXX, 11Z)
        11Z = (11B, XXX)
        22A = (22B, XXX)
        22B = (22C, 22C)
        22C = (22Z, 22Z)
        22Z = (22B, 22B)
        XXX = (XXX, XXX)
        """;
    
    
    [TestCase(SampleA, ExpectedResult = "2")]
    public string PartA_2Step(string input)
    {
        return new Day8Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(SampleB, ExpectedResult = "6")]
    public string PartA_6Step(string input)
    {
        return new Day8Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(SampleC, ExpectedResult = "6")]
    public string PartB_6Step(string input)
    {
        return new Day8Solution(input).Solve(SolutionPart.PartB);
    }
}