using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day3;

public class Day3Solution : Solution
{
    public Day3Solution()
    {
    }

    public Day3Solution(string input) : base(input)
    {
    }

    public override int Day => 3;

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => new Schematic(ReadInputLines()).SumSymbolNumbers().ToString(),
            SolutionPart.PartB => new Schematic(ReadInputLines()).SumGearRatios().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }
}

public class Schematic
{
    private CharMatrix Matrix { get; }

    public Schematic(List<string> lines)
    {
        Matrix = new CharMatrix(lines);
    }

    public int SumSymbolNumbers() =>
        Matrix.GetNumberObjects()
            .Where(n => n.HasSymbolInSurroundings(Matrix))
            .Select(n => int.Parse(n.Object))
            .Sum();

    public int SumGearRatios()
    {
        var numberObject = Matrix.GetNumberObjects();
        return Matrix.GetGearObjects()
            .Where(g => numberObject.Count(g.IsAdjacentTo) == 2)
            .Select(g => numberObject.Where(g.IsAdjacentTo).Aggregate(1, (i, o) => i * int.Parse(o.Object)))
            .Sum();
    }

    internal class CharMatrix
    {
        private List<string> Lines { get; }
        public DictionaryWithDefault<int, DictionaryWithDefault<int, char>> Matrix { get; }

        public CharMatrix(List<string> lines)
        {
            Lines = lines;
            Matrix = new DictionaryWithDefault<int, DictionaryWithDefault<int, char>>(
                new DictionaryWithDefault<int, char>('.'));

            for (var index = 0; index < lines.Count; index++)
            {
                Matrix[index] = new DictionaryWithDefault<int, char>('.');
                lines[index].ToCharArray()
                    .Select((c, i) => new KeyValuePair<int, char>(i, c))
                    .ToList()
                    .ForEach(e => Matrix[index][e.Key] = e.Value);
            }
        }

        public List<SchematicObject> GetNumberObjects()
            => GetObjectByRegex(new Regex(@"\d+"));

        public List<SchematicObject> GetGearObjects()
            => GetObjectByRegex(new Regex(@"\*"));

        public List<SchematicObject> GetObjectByRegex(Regex regex)
        {
            var objects = new List<SchematicObject>();
            for (var index = 0; index < Lines.Count; index++)
            {
                var line = Lines[index];
                var numberSequenceMatches = regex.Matches(line);
                foreach (Match numberSequenceMatch in numberSequenceMatches)
                {
                    objects.Add(new SchematicObject(
                        numberSequenceMatch.Value,
                        numberSequenceMatch.Index,
                        index
                    ));
                }
            }

            return objects;
        }
    }


    internal class SchematicObject
    {
        public string Object { get; }


        public int X { get; }
        public int Y { get; }

        public SchematicObject(string obj, int x, int y)
        {
            Object = obj;
            X = x;
            Y = y;
        }

        private List<Point> Surroundings()
        {
            var xMin = X - 1;
            var xMax = X + Object.Length;
            var yMin = Y - 1;
            var yMax = Y + 1;
            var surroundings = new List<Point>
            {
                new(xMin, Y), new(xMax, Y)
            };
            surroundings.AddRange(Enumerable.Range(xMin, xMax - xMin + 1).Select(x => new Point(x, yMin)));
            surroundings.AddRange(Enumerable.Range(xMin, xMax - xMin + 1).Select(x => new Point(x, yMax)));
            return surroundings;
        }

        private List<Point> InternalPoints()
            => Enumerable.Range(X, Object.Length).Select(x => new Point(x, Y)).ToList();


        public bool HasSymbolInSurroundings(CharMatrix matrix)
            => Surroundings()
                .Select(p => matrix.Matrix[p.Y][p.X])
                .Any(c => c != '.' && !char.IsDigit(c));

        public bool IsAdjacentTo(SchematicObject o)
            => Surroundings()
                .Any(p => o.InternalPoints().Any(i => p.X == i.X && p.Y == i.Y));
    }
}