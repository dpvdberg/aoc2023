using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day23;

namespace AdventOfCode2023Tests.Days;

public class Day23Tests
{
    private const string TestData =
        """
        #.#####################
        #.......#########...###
        #######.#########.#.###
        ###.....#.>.>.###.#.###
        ###v#####.#v#.###.#.###
        ###.>...#.#.#.....#...#
        ###v###.#.#.#########.#
        ###...#.#.#.......#...#
        #####.#.#.#######.#.###
        #.....#.#.#.......#...#
        #.#####.#.#.#########v#
        #.#...#...#...###...>.#
        #.#.#v#######v###.###v#
        #...#.>.#...>.>.#.###.#
        #####v#.#.###v#.#.###.#
        #.....#...#...#.#.#...#
        #.#########.###.#.#.###
        #...###...#...#...#.###
        ###.###.#.###v#####v###
        #...#...#.#.>.>.#.>.###
        #.###.###.#.###.#.#v###
        #.....###...###...#...#
        #####################.#
        """;

    private const string HomemadeTestData =
        """
        ####.####################
        ####.#######...........##
        ####.#######.#########.##
        ####.#######.#########.##
        ####.........###.......##
        ####.#####.#####.########
        ####.#####.#####.......##
        ####.#####.###########.##
        ####...................##
        ######################.##    
        """;

    private const string HomemadeTestData2 =
        """
        ####.####################
        ####.#######...........##
        ####.##...###############
        ####.##.#.############.##
        ####....#.######......###
        ####.####..#####.########
        ####.#####.#####.......##
        ####.#####.###########.##
        ####...................##
        ######################.##
        """;

    [TestCase(TestData, ExpectedResult = "94")]
    public string PartA(string input)
    {
        return new Day23Solution(input).Solve(SolutionPart.PartA);
    }
    
    [TestCase(TestData, ExpectedResult = "154")]
    public string PartB(string input)
    {
        return new Day23Solution(input).Solve(SolutionPart.PartB);
    }
    
    [TestCase(HomemadeTestData, ExpectedResult = "53")]
    public string PartB_homemade(string input)
    {
        return new Day23Solution(input).Solve(SolutionPart.PartB);
    }
    
    [TestCase(HomemadeTestData2, ExpectedResult = "31")]
    public string PartB_homemade2(string input)
    {
        return new Day23Solution(input).Solve(SolutionPart.PartB);
    }
}