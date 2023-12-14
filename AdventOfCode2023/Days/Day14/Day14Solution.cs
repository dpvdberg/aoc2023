using AdventOfCode2023.Common;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AdventOfCode2023.Days.Day14;

public class Day14Solution : Solution
{
    public override int Day => 14;

    public Day14Solution()
    {
    }

    public Day14Solution(string input) : base(input)
    {
    }

    private Matrix<double> Parse()
    {
        var m = Matrix<double>.Build;
        return m.DenseOfRows(ReadInputLines().Select(l => l.ToCharArray().Select(c => (double)c)));
    }

    public override string Solve(SolutionPart part)
    {
        var m = Parse();
        if (part == SolutionPart.PartA)
        {
            TiltNorth(m);
        }

        if (part == SolutionPart.PartB)
        {
            var builder = Matrix<double>.Build;
            var cache = new Dictionary<string, int>();
            
            var n = m.ColumnCount;
            var antiDiagonalIdentity = builder.Dense(n, n, 0);
            for (var i = 0; i <= n - 1; i++)
            {
                antiDiagonalIdentity[i, n - i - 1] = 1;
            }

            var iterations = 1000000000;
            bool cacheHit = false;
            for (int i = 0; i < iterations; i++)
            {

                if (!cacheHit && cache.ContainsKey(MatrixToString(m)))
                {
                    int cycleSize = i - cache[MatrixToString(m)];

                    // Remove cycles and continue
                    iterations = (iterations - i) % cycleSize;
                    i = 0;
                    cacheHit = true;
                }
                
                cache[MatrixToString(m)] = i;

                // North, East, South, West
                for (int j = 0; j < 4; j++)
                {
                    TiltNorth(m);

                    // rotate matrix
                    m = m.Transpose() * antiDiagonalIdentity;
                }
            }
            
        }
        
        return Count(m).ToString();
    }
    
    private static string MatrixToString(Matrix<double> matrix)
    {
        return string.Join(" ", matrix.EnumerateRows().SelectMany(x => x.Enumerate()));
    }
    
    private static int Count(Matrix<double> matrix)
    {
        var sum = 0;
        for (int row = 0; row < matrix.RowCount; row++)
        {
            var roundRocks = matrix.Row(row).Count(v => (RockType)v == RockType.Round);
            sum += roundRocks * (matrix.RowCount - row);
        }

        return sum;
    }

    private static void TiltNorth(Matrix<double> matrix)
    {
        while (TiltNorthStep(matrix))
        {
        }
    }

    private static bool TiltNorthStep(Matrix<double> matrix)
    {
        bool changed = false;
        for (int row = 1; row < matrix.RowCount; row++)
        {
            for (int col = 0; col < matrix.ColumnCount; col++)
            {
                if ((RockType)matrix[row, col] != RockType.Round)
                {
                    // Cannot move, skip
                    continue;
                }

                // Look up
                switch ((RockType)matrix[row - 1, col])
                {
                    case RockType.Round:
                        // Blocked
                        break;
                    case RockType.Square:
                        // Blocked
                        break;
                    case RockType.Empty:
                        matrix[row, col] = (double)RockType.Empty;
                        matrix[row - 1, col] = (double)RockType.Round;
                        changed = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        return changed;
    }

    public enum RockType
    {
        Round = 'O',
        Square = '#',
        Empty = '.'
    }
}