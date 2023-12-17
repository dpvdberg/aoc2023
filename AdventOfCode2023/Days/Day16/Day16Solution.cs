using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day16;

public class Day16Solution : Solution
{
    public override int Day => 16;

    public Day16Solution()
    {
    }

    public Day16Solution(string input) : base(input)
    {
    }

    private Tile[][] Parse() =>
        ReadInputLines().Select((l, y) =>
            l.ToCharArray().Select(c => c.ParseEnum<TileType>())
                .Select((t, x) => new Tile(t, x, y))
                .ToArray()
        ).ToArray();

    private class Tile
    {
        public int X { get; }
        public int Y { get; }
        public TileType Type { get; }
        public HashSet<Direction> LightDirections { get; } = new();

        public Tile(TileType type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        private Tile? GetTile(Direction d, Tile?[][] array) =>
            d switch
            {
                Direction.North => array.GetAtOrNull(Y - 1, X),
                Direction.East => array.GetAtOrNull(Y, X + 1),
                Direction.South => array.GetAtOrNull(Y + 1, X),
                Direction.West => array.GetAtOrNull(Y, X - 1),
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };

        public bool ApplyMirror(Direction d)
        {
            var startSize = LightDirections.Count;
            switch (Type)
            {
                case TileType.Empty:
                    LightDirections.Add(d);
                    break;
                case TileType.Horizontal:
                    if (d is Direction.North or Direction.South)
                    {
                        LightDirections.Add(Direction.East);
                        LightDirections.Add(Direction.West);
                    }
                    else
                    {
                        LightDirections.Add(d);
                    }
                    break;
                case TileType.Vertical:
                    if (d is Direction.West or Direction.East)
                    {
                        LightDirections.Add(Direction.South);
                        LightDirections.Add(Direction.North);
                    }
                    else
                    {
                        LightDirections.Add(d);
                    }
                    break;
                case TileType.BackSlash:
                    switch (d)
                    {
                        case Direction.North:
                            LightDirections.Add(Direction.West);
                            break;
                        case Direction.East:
                            LightDirections.Add(Direction.South);
                            break;
                        case Direction.South:
                            LightDirections.Add(Direction.East);
                            break;
                        case Direction.West:
                            LightDirections.Add(Direction.North);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(d), d, null);
                    }
                    break;
                case TileType.ForwardSlash:
                    switch (d)
                    {
                        case Direction.North:
                            LightDirections.Add(Direction.East);
                            break;
                        case Direction.East:
                            LightDirections.Add(Direction.North);
                            break;
                        case Direction.South:
                            LightDirections.Add(Direction.West);
                            break;
                        case Direction.West:
                            LightDirections.Add(Direction.South);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(d), d, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return LightDirections.Count != startSize;
        }

        public void Propagate(Tile[][] array)
        {
            var tiles = new List<Tile>();
            foreach (var dir in LightDirections)
            {
                var tile = GetTile(dir, array);
                if (tile == null) continue;
                
                if (tile.ApplyMirror(dir))
                {
                    tiles.Add(tile);
                }
            }

            foreach (var tile in tiles)
            {
                tile.Propagate(array);
            }
        }
    }

    private enum TileType
    {
        Empty = '.',
        Horizontal = '-',
        Vertical = '|',
        BackSlash = '\\',
        ForwardSlash = '/',
    }

    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    private int CountEnergized(Tile[][] array) =>
        array.SelectMany(r => r).Count(t => t.LightDirections.Count > 0);

    public override string Solve(SolutionPart part)
    {
        var tiles = Parse();
        if (part == SolutionPart.PartA)
        {
            var startTile = tiles[0][0];
        
            startTile.ApplyMirror(Direction.East);
            startTile.Propagate(tiles);
            return CountEnergized(tiles).ToString();
        }
        
        if (part == SolutionPart.PartB)
        {
            var max = 0;
            for (var row = 0; row < tiles.Length; row++)
            {
                var t = Parse();
                var startTile = t[row][0];
                startTile.ApplyMirror(Direction.East);
                startTile.Propagate(t);

                max = Math.Max(max, CountEnergized(t));
                
                t = Parse();
                startTile = t[row][^1];
                startTile.ApplyMirror(Direction.West);
                startTile.Propagate(t);

                max = Math.Max(max, CountEnergized(t));
            }
            
            for (var col = 0; col < tiles[0].Length; col++)
            {
                var t = Parse();
                var startTile = t[0][col];
                startTile.ApplyMirror(Direction.South);
                startTile.Propagate(t);

                max = Math.Max(max, CountEnergized(t));
                
                t = Parse();
                startTile = t[^1][col];
                startTile.ApplyMirror(Direction.North);
                startTile.Propagate(t);

                max = Math.Max(max, CountEnergized(t));
            }

            return max.ToString();
        }

        return "";
    }
}