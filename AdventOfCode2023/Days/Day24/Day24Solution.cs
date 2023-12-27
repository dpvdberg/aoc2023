using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using Vector = System.Numerics.Vector;

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

    private Matrix<double> CrossMatrix(Vector3D v)
    {
        return Matrix<double>.Build.DenseOfRowArrays(
            new[] { 0, -v.Z, v.Y },
            new[] { v.Z, 0, -v.X },
            new[] { -v.Y, v.X, 0 }
        );
    }

    public static Vector3D Cross(Vector3D left, Vector3D right)
    {
        Vector3D result = new Vector3D(
            left.Y * right.Z - left.Z * right.Y,
            -left.X * right.Z + left.Z * right.X,
            left.X * right.Y - left.Y * right.X
        );

        return result;
    }

    private double GetRockPositionSum()
    {
        var rays = Parse();
        var M = Matrix<double>.Build;

        // Could be any three rays
        var answerToLife = 42;
        var v0 = rays[answerToLife].Direction.ToVector3D();
        var v1 = rays[answerToLife + 1].Direction.ToVector3D();
        var v2 = rays[answerToLife + 2].Direction.ToVector3D();

        var p0 = rays[answerToLife].Position.ToVector3D();
        var p1 = rays[answerToLife + 1].Position.ToVector3D();
        var p2 = rays[answerToLife + 2].Position.ToVector3D();

        // Do some shifting to reduce size of positions
        var min = Vector3D.OfVector(p0.ToVector().PointwiseMinimum(p1.ToVector()).PointwiseMinimum(p2.ToVector()));
        p0 -= min;
        p1 -= min;
        p2 -= min;

        // Create matrix (M.x = y), see handwritten notes
        var matrix = M.DenseOfMatrixArray(new[,]
        {
            { CrossMatrix(v1) - CrossMatrix(v0), CrossMatrix(p0) - CrossMatrix(p1) },
            { CrossMatrix(v2) - CrossMatrix(v0) , CrossMatrix(p0) - CrossMatrix(p2) },
        });

        var topY = Cross(p0, v0) - Cross(p1, v1);
        var bottomY = Cross(p0, v0) - Cross(p2, v2);

        var y = topY.ToVector().ToColumnMatrix().Stack(bottomY.ToVector().ToColumnMatrix()).Column(0);

        var x = matrix.Solve(y);
        var pRock = new Vector3D(x[0], x[1], x[2]) + min;
        var vRock = new Vector3D(x[3], x[4], x[5]);

        return pRock.ToVector().Sum();
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

        public MathNet.Numerics.LinearAlgebra.Vector<double>? IntersectXY(Ray ray)
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

            var s = M.Inverse() * Vector<double>.Build.DenseOfArray(new double[]
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
        return part switch
        {
            SolutionPart.PartA => CountIntersectionsInArea(200000000000000, 400000000000000).ToString(),
            SolutionPart.PartB => GetRockPositionSum().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }
}