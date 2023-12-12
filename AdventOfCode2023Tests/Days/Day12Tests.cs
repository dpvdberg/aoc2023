using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day12;

namespace AdventOfCode2023Tests.Days.Day3;

public class Day12Tests
{
    [TestCase(
        """
        ???.### 1,1,3
        .??..??...?##. 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ????.######..#####. 1,6,5
        ?###???????? 3,2,1
        """, ExpectedResult = "21")]
    public string PartA(string input)
    {
        return new Day12Solution(input).Solve(SolutionPart.PartA);
    }

    [TestCase(
        """
        ..???#??.?????? 4,3
        """, ExpectedResult = "12")]
    public string PartA_single(string input)
    {
        return new Day12Solution(input).Solve(SolutionPart.PartA);
    }

    [TestCase(
        """
        ?##??##???.??.?#? 3,4,1,2,2
        """, ExpectedResult = "2")]
    public string PartA_long(string input)
    {
        return new Day12Solution(input).Solve(SolutionPart.PartA);
    }

    [TestCase(
        """
        ???.### 1,1,3
        """, ExpectedResult = "1")]
    public string PartB_simple(string input)
    {
        return new Day12Solution(input).Solve(SolutionPart.PartB);
    }

    [TestCase(
        """
        ????.#...#... 4,1,1
        """, ExpectedResult = "16")]
    public string PartB(string input)
    {
        return new Day12Solution(input).Solve(SolutionPart.PartB);
    }

    [TestCase(
        """
        ???.### 1,1,3
        .??..??...?##. 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ????.######..#####. 1,6,5
        ?###???????? 3,2,1
        """, ExpectedResult = "525152")]
    public string PartB_large(string input)
    {
        return new Day12Solution(input).Solve(SolutionPart.PartB);
    }
}