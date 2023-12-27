using System.Collections.Immutable;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;
using MathNet.Numerics;

namespace AdventOfCode2023.Days.Day23;

public class Day23Solution : Solution
{
    public Day23Solution()
    {
    }

    public Day23Solution(string input) : base(input)
    {
    }

    public override int Day => 23;

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse(part).GetPathsNaive().Max(p => p.Count() - 1).ToString(),
            SolutionPart.PartB => Parse(part).GetMaxPath().ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private Map Parse(SolutionPart part)
        => new(ReadInputLines().Select((l, y) =>
            l.ToCharArray().Select((c, x) =>
                new Tile(c.ParseEnum<TileType>(), new Vec2(x, y), part != SolutionPart.PartA)
            ).ToArray()
        ).ToArray());

    private class Map
    {
        public Tile[][] Tiles { get; }

        public Map(Tile[][] tiles)
        {
            Tiles = tiles;
        }

        public Tile GetStart() => Tiles[0].FirstOrDefault(t => t.Type == TileType.Path)!;

        public bool IsEnd(Tile t) => t.Coord.Y == Tiles.Length - 1 && t.Type == TileType.Path;

        public bool IsFork(Tile t) => t.GetNeighbors(this).Count > 2;

        private List<Node> BuildForkGraph()
        {
            var startNode = new Node(GetStart());
            var currentNode = startNode;

            var nodes = new List<Node>() { currentNode };
            var stack = new Stack<(Tile, Node, int)>();
            var visited = new HashSet<Tile>();

            stack.Push((GetStart(), startNode, 0));
            visited.Add(GetStart());

            while (stack.Count > 0)
            {
                var (tile, intersection, steps) = stack.Pop();
                visited.Add(tile);

                foreach (var t in tile.GetNeighbors(this))
                {
                    if (IsEnd(t) || IsFork(t))
                    {
                        Node? node = nodes.FirstOrDefault(n => n.Tile == t);
                        if (node == null)
                        {
                            node = new Node(t);
                            nodes.Add(node);
                        }

                        // Bi-directional edge
                        intersection.AddChild(node, steps + 1);
                        node.AddChild(intersection, steps + 1);

                        if (!visited.Contains(t))
                        {
                            stack.Push((t, node, 0));
                        }
                    }
                    else if (!visited.Contains(t))
                    {
                        stack.Push((t, intersection, steps + 1));
                    }
                }
            }

            return nodes;
        }

        public int GetMaxPath()
        {
            var forkGraph = BuildForkGraph();
            var paths = GetForkGraphPaths(forkGraph);

            var maxDistance = 0;
            foreach (var path in paths)
            {
                var distance = 0;
                var previous = path.First();
                foreach (var node in path.Skip(1))
                {
                    distance += previous.Childs[node];
                    previous = node;
                }

                Console.WriteLine($"[{distance}] = {String.Join(",", path.Select(n => n.Tile.Coord))}");
                maxDistance = Math.Max(maxDistance, distance);
            }

            return maxDistance;
        }

        public List<ImmutableStack<Node>> GetForkGraphPaths(List<Node> forkGraph)
        {
            var startNode = forkGraph.FirstOrDefault(n => n.Tile == GetStart())!;

            var s = new Stack<ImmutableStack<Node>>();
            s.Push(ImmutableStack<Node>.Empty.Push(startNode));

            var l = new List<ImmutableStack<Node>>();

            while (s.Count > 0)
            {
                var path = s.Pop();

                var current = path.Peek();

                if (IsEnd(current.Tile))
                {
                    l.Add(path);
                    continue;
                }

                foreach (var (child, distance) in current.Childs)
                {
                    if (!path.Contains(child))
                    {
                        s.Push(path.Push(child));
                    }
                }
            }

            return l;
        }

        public List<ImmutableStack<Tile>> GetPathsNaive()
        {
            var s = new Stack<ImmutableStack<Tile>>();
            s.Push(ImmutableStack<Tile>.Empty.Push(GetStart()));

            var l = new List<ImmutableStack<Tile>>();

            while (s.Count > 0)
            {
                var path = s.Pop();

                var current = path.Peek();

                if (IsEnd(current))
                {
                    l.Add(path);
                    continue;
                }

                foreach (var tile in current.GetNeighbors(this))
                {
                    if (!path.Contains(tile))
                    {
                        s.Push(path.Push(tile));
                    }
                }
            }

            return l;
        }

        public Tile GetAt(Vec2 coord) => Tiles[coord.Y][coord.X];

        public bool IsValid(Vec2 coord) =>
            coord.Y >= 0 && coord.Y < Tiles.Length && coord.X >= 0 && coord.X < Tiles[0].Length;
    }

    private class Tile
    {
        private readonly bool _allowAllSlopes;
        public TileType Type { get; }
        public Vec2 Coord { get; }

        public Tile(TileType type, Vec2 coord, bool allowAllSlopes)
        {
            _allowAllSlopes = allowAllSlopes;
            Type = type;
            Coord = coord;
        }

        public List<Tile> GetNeighbors(Map map)
        {
            var l = new List<Tile>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var coordinate = Coord + DirectionVec[direction];
                if (!map.IsValid(coordinate))
                {
                    continue;
                }

                var tile = map.GetAt(coordinate);
                var allowedTypes = _allowAllSlopes ? AllowedTypesFull : AllowedTypes;
                if (allowedTypes[direction].Contains(tile.Type))
                {
                    l.Add(tile);
                }
            }

            return l;
        }
    }

    private class Node
    {
        public Tile Tile { get; }

        public Node(Tile tile)
        {
            Tile = tile;
        }

        public DictionaryWithDefault<Node, int> Childs { get; } = new(0);

        public void AddChild(Node node, int steps)
        {
            Childs[node] = Math.Max(Childs[node], steps);
        }
    }

    private static readonly Dictionary<Direction, List<TileType>> AllowedTypes = new()
    {
        { Direction.North, new List<TileType>() { TileType.Path, TileType.SlopeNorth } },
        { Direction.East, new List<TileType>() { TileType.Path, TileType.SlopeEast } },
        { Direction.South, new List<TileType>() { TileType.Path, TileType.SlopeSouth } },
        { Direction.West, new List<TileType>() { TileType.Path, TileType.SlopeWest } },
    };

    private static readonly Dictionary<Direction, List<TileType>> AllowedTypesFull = new()
    {
        {
            Direction.North,
            new List<TileType>()
                { TileType.Path, TileType.SlopeNorth, TileType.SlopeEast, TileType.SlopeSouth, TileType.SlopeWest }
        },
        {
            Direction.East,
            new List<TileType>()
                { TileType.Path, TileType.SlopeNorth, TileType.SlopeEast, TileType.SlopeSouth, TileType.SlopeWest }
        },
        {
            Direction.South,
            new List<TileType>()
                { TileType.Path, TileType.SlopeNorth, TileType.SlopeEast, TileType.SlopeSouth, TileType.SlopeWest }
        },
        {
            Direction.West,
            new List<TileType>()
                { TileType.Path, TileType.SlopeNorth, TileType.SlopeEast, TileType.SlopeSouth, TileType.SlopeWest }
        },
    };

    private static readonly Dictionary<Direction, Vec2> DirectionVec = new()
    {
        { Direction.North, new Vec2(0, -1) },
        { Direction.East, new Vec2(1, 0) },
        { Direction.South, new Vec2(0, 1) },
        { Direction.West, new Vec2(-1, 0) },
    };

    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    private enum TileType
    {
        Path = '.',
        Forest = '#',
        SlopeNorth = '^',
        SlopeEast = '>',
        SlopeSouth = 'v',
        SlopeWest = '<'
    }
}