using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day9;

namespace AdventOfCode2023Tests.Days.Day3;

public class Day9Tests
{
    [TestCase("0 3 6 9 12 15", ExpectedResult = "18")]
    public string PartA_Single(string input)
    {
        return new Day9Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase("1 3 6 10 15 21", ExpectedResult = "28")]
    public string PartA_Single2(string input)
    {
        return new Day9Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase("14 13 12 11 10 9 8 7 6 5 4 3 2 1 0 -1 -2 -3 -4 -5 -6", ExpectedResult = "-7")]
    public string PartA_SingleNegative(string input)
    {
        return new Day9Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(
        """
        0 3 6 9 12 15
        1 3 6 10 15 21
        10 13 16 21 30 45
        """, ExpectedResult = "114")]
    public string PartA_Multi(string input)
    {
        return new Day9Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase("10  13  16  21  30  45", ExpectedResult = "5")]
    public string PartB_Single(string input)
    {
        return new Day9Solution(input).Solve(SolutionPart.PartB);
    }
}