using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;
using Google.OrTools.LinearSolver;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AdventOfCode2023.Days.Day24;

public class Day24Solution : Solution
{
    public override int Day => 24;

    public Day24Solution()
    {
    }

    public Day24Solution(string input) : base(input)
    {
    }

    private List<Ray> Parse()
        => ReadInputLines()
            .Select(l => l.Split('@'))
            .Select(s => (s[0].Split(','), s[1].Split(',')))
            .Select(t => new Ray(
                new Vec3(long.Parse(t.Item1[0]), long.Parse(t.Item1[1]), long.Parse(t.Item1[2])),
                new Vec3(long.Parse(t.Item2[0]), long.Parse(t.Item2[1]), long.Parse(t.Item2[2]))
            )).ToList();

    private long GetRockPosition()
    {
        // todo part 2

    }

    private long CountIntersectionsInArea(long min, long max)
    {
        var rays = Parse();
        var pairs = new List<(Ray, Ray)>();
        for (int i = 0; i < rays.Count; i++)
        {
            for (int j = i + 1; j < rays.Count; j++)
            {
                pairs.Add((rays[i], rays[j]));
            }
        }

        var intersections = pairs.Select(p => p.Item1.IntersectXY(p.Item2)).Where(i => i != null);

        return intersections.Count(i => i[0] >= min && i[0] <= max && i[1] >= min && i[1] <= max);
    }
    

    private class Ray
    {
        public Vec3 Position { get; }
        public Vec3 Direction { get; }

        public Ray(Vec3 position, Vec3 direction)
        {
            Position = position;
            Direction = direction;
        }
        
        public bool SameSign(double num1, double num2)
        {
            return num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0;
        }

        public Vector<double>? IntersectXY(Ray ray)
        {
            Matrix<double> M = Matrix.Build.DenseOfRowArrays(
                new double[] { Direction.X, -ray.Direction.X },
                new double[] { Direction.Y, -ray.Direction.Y }
            );

            var det = M.Determinant();

            if (det == 0)
            {
                return null;
            }

            var s = M.Inverse() * Vector.Build.DenseOfArray(new double[]
            {
                ray.Position.X - Position.X,
                ray.Position.Y - Position.Y
            });

            var intersection = Position.ToMathVector() + s[0] * Direction.ToMathVector();
            var diff = intersection - Position.ToMathVector();
            for (int i = 0; i < 2; i++)
            {
                if (!SameSign(diff[i], Direction[i]))
                {
                    return null;
                }
            }

            diff = intersection - ray.Position.ToMathVector();
            for (int i = 0; i < 2; i++)
            {
                if (!SameSign(diff[i], ray.Direction[i]))
                {
                    return null;
                }
            }
            
            return intersection;
        }
    }

    public override string Solve(SolutionPart part)
    {
        return CountIntersectionsInArea(200000000000000, 400000000000000).ToString();
    }
}