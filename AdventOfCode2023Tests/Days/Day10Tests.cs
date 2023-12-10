using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day10;

namespace AdventOfCode2023Tests.Days.Day3;

public class Day10Tests
{
    [TestCase(
        """
        .....
        .S-7.
        .|.|.
        .L-J.
        .....
        """, ExpectedResult = "4")]
    public string PartA_Simple(string input)
    {
        return new Day10Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(
        """
        -L|F7
        7S-7|
        L|7||
        -L-J|
        L|-JF
        """, ExpectedResult = "4")]
    public string PartA_SimpleCluttered(string input)
    {
        return new Day10Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(
        """
        ..F7.
        .FJ|.
        SJ.L7
        |F--J
        LJ...
        """, ExpectedResult = "8")]
    public string PartA_Longer(string input)
    {
        return new Day10Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(
        """
        ...........
        .S-------7.
        .|F-----7|.
        .||.....||.
        .||.....||.
        .|L-7.F-J|.
        .|..|.|..|.
        .L--J.L--J.
        ...........
        """, ExpectedResult = "4")]
    public string PartB_Small(string input)
    {
        return new Day10Solution(input).Solve(SolutionPart.PartB);
    }
    
    [TestCase(
        """
        .F----7F7F7F7F-7....
        .|F--7||||||||FJ....
        .||.FJ||||||||L7....
        FJL7L7LJLJ||LJ.L-7..
        L--J.L7...LJS7F-7L7.
        ....F-J..F7FJ|L7L7L7
        ....L7.F7||L7|.L7L7|
        .....|FJLJ|FJ|F7|.LJ
        ....FJL-7.||.||||...
        ....L---J.LJ.LJLJ...
        """, ExpectedResult = "8")]
    public string PartB_Large(string input)
    {
        return new Day10Solution(input).Solve(SolutionPart.PartB);
    }
}