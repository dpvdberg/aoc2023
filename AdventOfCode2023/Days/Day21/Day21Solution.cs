using System.Diagnostics;
using System.Numerics;
using System.Reflection.PortableExecutable;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day21;

public class Day21Solution : Solution
{
    public override int Day => 21;
    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => new Day21SolutionNaive(false).Parse(false).BFS().Count(v => v.Value <= 64 && v.Value % 2 == 0).ToString(),
            SolutionPart.PartB => PartB(26501365),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    public string PartB(int steps)
    {
        var naive = new Day21SolutionNaive(false);
        var map = naive.Parse(true);

        var height = map.Grid.Length;
        var width = map.Grid[0].Length;

        var start = map.GetStart();

        // From the visualization of the naive solution (and the input itself), we can make a number of observations.
        // 1. The input is square
        Debug.Assert(width == height);
        // 2. There are no rocks on the horizontal and vertical axis, so we grow in a diamond.
        var startRow = map.Grid[start.Coord.Y].ToList();
        Debug.Assert(startRow.All(t => t.Type != Day21SolutionNaive.MapTileType.Rock));
        var startColumn = Enumerable.Range(0, height).Select(y => map.Grid[y][start.Coord.X]).ToList();
        Debug.Assert(startColumn.All(t => t.Type != Day21SolutionNaive.MapTileType.Rock));
        // Let rho be the number of input squares we traverse in one direction using the given amount of steps
        var inscribedDiamondWidth = start.Coord.X;
        var rho = (BigInteger) (steps - inscribedDiamondWidth) / width;
        // 3. Input size is odd, meaning we create a checkerboard pattern for each crossed input border
        Debug.Assert(width % 2 == 1);
        
        // Lets completely saturate the base input
        var tempMap = naive.Parse(false);
        var distanceDict = tempMap.BFS();

        // Number of reachable odd / even tiles in for one input square
        var odd = (BigInteger) distanceDict.Count(d => d.Value % 2 == 1);
        var even = (BigInteger) distanceDict.Count(d => d.Value % 2 == 0);
        
        // Number of reachable odd / even tiles in for one input square outside the inscribed diamond
        var cornerOdd = (BigInteger) distanceDict.Count(d => d.Value % 2 == 1 && d.Value > inscribedDiamondWidth);
        var cornerEven = (BigInteger) distanceDict.Count(d => d.Value % 2 == 0 && d.Value > inscribedDiamondWidth);

        var result = 
              (rho + 1) * (rho + 1) * odd  // Fully covered odd squares
            + rho * rho * even // Fully covered even squares
            - (rho + 1) * cornerOdd // Over-estimated corners caused by diamond pattern
            + rho * cornerEven; // Under-estimated corners caused by diamond pattern

        return result.ToString();
    }
}