namespace AdventOfCode2023.Utils;

public sealed class Vec2 : IEquatable<Vec2>
{
    public long X { get; }
    public long Y { get; }

    public Vec2(long x, long y)
    {
        X = x;
        Y = y;
    }
    public static Vec2 operator +(Vec2 a, Vec2 b)
        => new(a.X + b.X, a.Y + b.Y);

    public bool Equals(Vec2? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Vec2)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}


public sealed class Vec2Double : IEquatable<Vec2Double>
{
    public double X { get; }
    public double Y { get; }

    public Vec2Double(double x, double y)
    {
        X = x;
        Y = y;
    }
    public static Vec2Double operator +(Vec2Double a, Vec2Double b)
        => new(a.X + b.X, a.Y + b.Y);

    public bool Equals(Vec2Double? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Math.Abs(X - other.X) < 1e-4 && Math.Abs(Y - other.Y) < 1e-4;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Vec2)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}