using System.Globalization;
using AdventOfCode2023.Common;
using MathNet.Numerics.LinearAlgebra;

namespace AdventOfCode2023.Days.Day18;

public class Day18Solution : Solution
{
    public Day18Solution()
    {
    }

    public Day18Solution(string input) : base(input)
    {
    }

    private List<Vector<double>> Parse(SolutionPart part)
    {
        var V = Vector<double>.Build;
        // Get dimensions
        List<Vector<double>> vectors;
        if (part == SolutionPart.PartA)
        {
            vectors = ReadInputLines().Select(l => l.Split(' '))
                .Select(s =>
                {
                    var direction = s[0][0];
                    var length = double.Parse(s[1]);
                    return direction switch
                    {
                        'R' => V.DenseOfArray(new[] { length, 0 }),
                        'L' => V.DenseOfArray(new[] { -length, 0 }),
                        'U' => V.DenseOfArray(new[] { 0, -length }),
                        'D' => V.DenseOfArray(new[] { 0, length }),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }).ToList();
        }
        else
        {
            // part B
            vectors = ReadInputLines().Select(l => l.Split(' '))
                .Select(s =>
                {
                    var hex = s.Last().Trim('(', ')', '#');
                    var direction = hex.Last();
                    var length = (double)int.Parse(hex.Remove(hex.Length - 1), NumberStyles.HexNumber);
                    return direction switch
                    {
                        '0' => V.DenseOfArray(new[] { length, 0 }), // R
                        '1' => V.DenseOfArray(new[] { 0, length }), // D
                        '2' => V.DenseOfArray(new[] { -length, 0 }), // L
                        '3' => V.DenseOfArray(new[] { 0, -length }), // U
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }).ToList();
        }

        // Find polygon lattice points
        var path = new List<Vector<double>>();
        var current = V.DenseOfArray(new[] { 0.0, 0.0 });
        foreach (var v in vectors)
        {
            current += v;
            path.Add(current.Clone());
        }

        return path.ToList();
    }

    public override int Day => 18;

    static double HullArea(List<Vector<double>> v)
    {
        // Shoelace formula
        var shiftedVertices = v.Skip(1).Append(v[0]).ToList();
        var product = v.Zip(shiftedVertices, (p1, p2) => p1[0] * p2[1] - p1[1] * p2[0]);
        var area = Math.Abs(product.Sum()) / 2;

        // Pick's theorem
        var edge = v.Zip(shiftedVertices, (p1, p2) => (p1-p2).L2Norm()).Sum();
        var interior = area - edge / 2 + 1;

        return edge + interior;
    }

    public override string Solve(SolutionPart part)
    {
        return HullArea(Parse(part)).ToString();
    }
}