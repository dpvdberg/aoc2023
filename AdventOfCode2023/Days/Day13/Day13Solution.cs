using AdventOfCode2023.Common;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace AdventOfCode2023.Days.Day13;

public class Day13Solution : Solution
{
    public Day13Solution()
    {
    }

    public Day13Solution(string input) : base(input)
    {
    }

    public override int Day => 13;

    private List<Matrix<double>> Parse()
    {
        var m = Matrix<double>.Build;
        var rawMatrix = ReadInput()
            .Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(r =>
                m.DenseOfRows(
                    r.Split("\r\n")
                        .Select(l => l.ToCharArray().Select(c => c == '#' ? 1.0 : 0.0)))
            ).ToList();

        return rawMatrix;
    }

    private int GetRowMirrorPosition(Matrix<double> matrix)
    {
        // if mirror == i, then we expect the mirror to be in between rows i and i + 1
        for (int mirror = 0; mirror < matrix.RowCount - 1; mirror++)
        {
            var maxOffset = Math.Min(mirror, matrix.RowCount - mirror - 2);
            bool mirrorOk = true;
            for (int offset = 0; offset <= maxOffset; offset++)
            {
                if (!matrix.Row(mirror - offset).Equals(matrix.Row(mirror + offset + 1)))
                {
                    mirrorOk = false;
                    break;
                }
            }

            if (mirrorOk)
            {
                return mirror + 1;
            }
        }

        return -1;
    }

    private int GetSmudgedRowMirrorPosition(Matrix<double> matrix)
    {
        // if mirror == i, then we expect the mirror to be in between rows i and i + 1
        for (int mirror = 0; mirror < matrix.RowCount - 1; mirror++)
        {
            var maxOffset = Math.Min(mirror, matrix.RowCount - mirror - 2);
            bool smudgeFound = false;
            for (int offset = 0; offset <= maxOffset; offset++)
            {
                var rowDiffVector = matrix.Row(mirror - offset) - matrix.Row(mirror + offset + 1);
                var rowDiff = rowDiffVector.PointwiseAbs().Sum();
                if (rowDiff.AlmostEqual(1)) // Single mismatch in mirrored rows 
                {
                    if (smudgeFound)
                    {
                        // Single smudge already found
                        smudgeFound = false;
                        break;
                    }
    
                    // Assume this is the smudge
                    smudgeFound = true;
                }
                else if (rowDiff > 1)
                {
                    smudgeFound = false;
                    break;
                }
            }

            if (smudgeFound)
            {
                return mirror + 1;
            }
        }

        return -1;
    }

    private long Compute(Matrix<double> matrix, Func<Matrix<Double>, int> f)
    {
        var rowPosition = f(matrix);
        if (rowPosition >= 0)
        {
            return rowPosition * 100;
        }

        var columnPosition = f(matrix.Transpose());
        if (columnPosition >= 0)
        {
            return columnPosition;
        }

        throw new ArgumentException("No mirror found in matrix");
    }

    private long Compute(SolutionPart part) => Parse().Sum(m => Compute(m,
        matrix => part == SolutionPart.PartA ? GetRowMirrorPosition(matrix) : GetSmudgedRowMirrorPosition(matrix)
    ));

    public override string Solve(SolutionPart part)
    {
        return Compute(part).ToString();
    }
}