using System.Drawing;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day10;

public class Day10Solution : Solution
{
    public Day10Solution()
    {
    }

    public Day10Solution(string input) : base(input)
    {
    }

    public override int Day => 10;

    private Field Parse()
        => new Field(ReadInputLines().Select((l, i) =>
            l.ToCharArray().Select((c, j) =>
                new Pipe(new Point(j, i), c.ParseEnum<PipeType>())
            ).ToArray()
        ).ToArray());

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse().GetMaxDistancePipeInLoop().ToString(),
            SolutionPart.PartB => Parse().GetHullSize().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private class Field
    {
        private readonly Pipe[][] _rawMatrix;
        public DictionaryWithDefault<int, DictionaryWithDefault<int, Pipe>> Matrix { get; }

        public List<Pipe> AllPipes => _rawMatrix.SelectMany(p => p).ToList();

        public Field(Pipe[][] rawMatrix)
        {
            this._rawMatrix = rawMatrix;
            Matrix = new DictionaryWithDefault<int, DictionaryWithDefault<int, Pipe>>(
                new DictionaryWithDefault<int, Pipe>(new Pipe(Point.Empty, PipeType.Ground)));

            for (var index = 0; index < rawMatrix.Length; index++)
            {
                var pl = rawMatrix[index];
                Matrix[index] = new DictionaryWithDefault<int, Pipe>(new Pipe(Point.Empty, PipeType.Ground));

                foreach (var pipe in pl)
                {
                    Matrix[pipe.Coord.Y][pipe.Coord.X] = pipe;
                }
            }
        }

        public int GetMaxDistancePipeInLoop() =>
            (int)Math.Floor((double)DFS(GetStart()).Count / 2);

        private int CountCrossings(List<Pipe> pipes)
        {
            var groups = pipes
                .SelectMany(p => Pipe.GetDirectionForType(p.Type))
                .ToLookup(d => d);

            if (!groups[Direction.Down].Any() || !groups[Direction.Up].Any())
            {
                return 0;
            }

            return Math.Min(groups[Direction.Down].Count(), groups[Direction.Up].Count());
        }

        public int GetHullSize()
        {
            var loop = DFS(GetStart());
            var remainingPipes = AllPipes.Except(loop);

            // Use ray algorithm,
            // If number of loop pipes to the left of a point in the field is uneven, it is in the hull

            int count = 0;
            foreach (var pipe in remainingPipes)
            {
                var leftPipes = _rawMatrix[pipe.Coord.Y]
                    .Where(loop.Contains)
                    .Where(p => p.Coord.X < pipe.Coord.X).ToList();
                var leftCount = leftPipes.Count > 0 ? CountCrossings(leftPipes) : 0;

                var rightPipes = _rawMatrix[pipe.Coord.Y]
                    .Where(loop.Contains)
                    .Where(p => p.Coord.X > pipe.Coord.X).ToList();
                var rightCount = rightPipes.Count > 0 ? CountCrossings(rightPipes) : 0;

                if (leftCount % 2 == 1 && rightCount > 0)
                {
                    count++;
                }
                else if (rightCount % 2 == 1 && leftCount > 0)
                {
                    count++;
                }
            }

            return count;
        }

        public List<Pipe> DFS(Pipe start)
        {
            var visisted = new HashSet<Pipe>();
            var stack = new Stack<Pipe>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visisted.Contains(current))
                {
                    visisted.Add(current);

                    var connected = current.GetConnectedPipes(this);
                    foreach (var p in connected)
                    {
                        if (p.Type == PipeType.Start)
                        {
                            // Prevent loop of size 1
                            if (current.Parent!.Type != PipeType.Start)
                            {
                                p.Parent = current;
                                return p.GetParentChain();
                            }
                        }
                        else if (!visisted.Contains(p))
                        {
                            p.Parent = current;
                            stack.Push(p);
                        }
                    }
                }
            }

            return new List<Pipe>();
        }

        private Pipe GetStart() => AllPipes.FirstOrDefault(p => p.Type == PipeType.Start)!;
    }

    private class Pipe
    {
        public Pipe? Parent { get; set; } = null;
        public Point Coord { get; }
        public PipeType Type { get; }

        public Pipe(Point coord, PipeType type)
        {
            Coord = coord;
            Type = type;
        }

        public List<Pipe> GetParentChain()
        {
            var l = new List<Pipe>();

            var current = this;
            do
            {
                l.Add(current);
                current = current.Parent;
            } while (current != null && current != this);

            return l;
        }

        public List<Pipe> GetConnectedPipes(Field field) =>
            GetDirectionForType(Type)
                .Select(d => (d, GetAdjacentPoints()[d]))
                .Select(p => (p.d, field.Matrix[p.Item2.Y][p.Item2.X]))
                .Where(p => GetDirectionForType(p.Item2.Type).Contains(GetOppositeDirection(p.d)))
                .Select(p => p.Item2).ToList();

        private Dictionary<Direction, Point> GetAdjacentPoints() => new()
        {
            { Direction.Down, Coord with { Y = Coord.Y + 1 } },
            { Direction.Up, Coord with { Y = Coord.Y - 1 } },
            { Direction.Right, Coord with { X = Coord.X + 1 } },
            { Direction.Left, Coord with { X = Coord.X - 1 } },
        };

        private static List<PipeType> GetTypesForDirection(Direction d) =>
            d switch
            {
                Direction.Up => new List<PipeType>
                    { PipeType.Start, PipeType.Vertical, PipeType.NorthEast, PipeType.NorthWest },
                Direction.Down => new List<PipeType>
                    { PipeType.Start, PipeType.Vertical, PipeType.SouthEast, PipeType.SouthWest },
                Direction.Left => new List<PipeType>
                    { PipeType.Start, PipeType.Horizontal, PipeType.NorthWest, PipeType.SouthWest },
                Direction.Right => new List<PipeType>
                    { PipeType.Start, PipeType.Horizontal, PipeType.NorthEast, PipeType.SouthEast },
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };


        public static List<Direction> GetDirectionForType(PipeType p) =>
            Enum.GetValues(typeof(Direction)).Cast<Direction>()
                .Where(d => GetTypesForDirection(d).Contains(p))
                .ToList();


        private static Direction GetOppositeDirection(Direction d) =>
            d switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
    }

    private enum PipeType
    {
        Vertical = '|',
        Horizontal = '-',
        NorthEast = 'L',
        NorthWest = 'J',
        SouthWest = '7',
        SouthEast = 'F',
        Ground = '.',
        Start = 'S'
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}