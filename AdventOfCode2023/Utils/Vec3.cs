using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AdventOfCode2023.Utils;

public sealed class Vec3 : IEquatable<Vec3>
{
    public long X { get; }
    public long Y { get; }
    public long Z { get; }

    public Vec3(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public long this[int key] => new[] { X, Y, Z }[key];

    public Vector<double> ToMathVector()
    {
        return Vector.Build.DenseOfArray(new double[] { X, Y, Z });
    }

    public Vec2 GetXY() => new(X, Y);

    public static Vec3 operator +(Vec3 a, Vec3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vec3 operator -(Vec3 a, Vec3 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vec3 operator *(Vec3 a, long b)
        => new(a.X * b, a.Y * b, a.Z * b);

    public static Vec3 operator /(Vec3 a, long b)
        => new(a.X / b, a.Y / b, a.Z / b);

    public long L1Norm() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

    public static IEnumerable<Vec3> operator |(Vec3 a, Vec3 b)
    {
        // interpolation
        var diff = b - a;
        var length = diff.L1Norm();

        if (length <= 0)
        {
            return new[] { a };
        }

        var unit = diff / length;

        return Enumerable.Range(0, (int)length + 1)
            .Select(d => a + (unit * d))
            .ToArray();
    }

    public bool Equals(Vec3? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}