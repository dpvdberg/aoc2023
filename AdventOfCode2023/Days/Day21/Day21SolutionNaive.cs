using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace AdventOfCode2023.Days.Day21;

public class Day21SolutionNaive : Solution
{
    private readonly bool _visualize;
    public override int Day => 21;

    public Day21SolutionNaive(bool visualize)
    {
        _visualize = visualize;
    }

    public Day21SolutionNaive(string input, bool visualize) : base(input)
    {
        _visualize = visualize;
    }

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            // SolutionPart.PartA => Parse(false).Walk(64).ToString(),
            SolutionPart.PartA => Parse(false).BFS().Count(v => v.Value <= 64 && v.Value % 2 == 0).ToString(),
            // This just takes forever...
            SolutionPart.PartB => Parse(true).Walk(26501365).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    public Map Parse(bool allowWrap)
    {
        var array = ReadInputLines().Select(
            (l, y) => l.ToCharArray()
                .Select((c, x) => new Tile(c, x, y))
                .ToArray()
        ).ToArray();

        return new Map(array, allowWrap, _visualize);
    }

    public class Map
    {
        private readonly bool _visualize;
        public Tile[][] Grid { get; }
        public bool AllowRepeat { get; }

        public Map(Tile[][] grid, bool allowRepeat, bool visualize)
        {
            _visualize = visualize;
            Grid = grid;
            AllowRepeat = allowRepeat;
        }

        public int Walk(int steps)
        {
            // This is just really slow.....
            // Get start and modify it to make it a plot
            var start = GetStart();
            var startPlot = new Tile(MapTileType.Plot, start.Coord);
            SetAt(start.Coord, startPlot);

            var queue = new Queue<Tile>();
            queue.Enqueue(startPlot);

            for (int step = 0; step < steps; step++)
            {
                var newQueue = new Queue<Tile>();
                while (queue.Count > 0)
                {
                    var tile = queue.Dequeue();
                    var neighbors = tile.GetNeighbors(this);
                    foreach (var n in neighbors)
                    {
                        if (!newQueue.Contains(n))
                        {
                            newQueue.Enqueue(n);
                        }
                    }
                }

                if (_visualize)
                {
                    VisualizeMap(newQueue.ToList(), step);
                }

                queue = newQueue;
            }

            return queue.Count;
        }

        public Dictionary<Tile, int> BFS()
        {
            var d = new Dictionary<Tile, int>();
            
            var start = GetStart();
            var startPlot = new Tile(MapTileType.Plot, start.Coord);
            SetAt(start.Coord, startPlot);

            var visited = new HashSet<Tile>();
            var queue = new Queue<Tile>();
            
            queue.Enqueue(startPlot);
            d[startPlot] = 0;
            
            while (queue.Count > 0)
            {
                var tile = queue.Dequeue();
                visited.Add(tile);

                var neighbors = tile.GetNeighbors(this);
                foreach (var n in neighbors)
                {
                    if (!visited.Contains(n) && !queue.Contains(n))
                    {
                        queue.Enqueue(n);
                        d[n] = d[tile] + 1;
                    }
                }
            }

            return d;
        }

        private void VisualizeMap(List<Tile> tiles, int steps)
        {
            var model = new PlotModel { Title = $"Step = {steps}, #tiles = {tiles.Count}" };

            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(100)
            });


            var xMin = tiles.Min(t => t.Coord.X);
            var xMax = tiles.Max(t => t.Coord.X);
            var yMin = tiles.Min(t => t.Coord.Y);
            var yMax = tiles.Max(t => t.Coord.Y);

            var height = Grid.Length;
            var width = Grid[0].Length;
            
            var data = new double[yMax - yMin + 1, xMax - xMin + 1];
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    
                    var tile = GetAt(new Coordinate(x, y));

                    if (Mod(x, width) == 0 || Mod(y, height) == 0)
                    {
                        data[y - yMin, x - xMin] = 5;
                        continue;
                    }
                    
                    data[y - yMin, x - xMin] =
                        tiles.Contains(tile)
                            ? 10
                            : tile.Type == MapTileType.Rock
                                ? -10
                                : 0;
                }
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = xMin,
                X1 = xMax,
                Y0 = yMin,
                Y1 = yMax,
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Rectangles,
                Data = data
            };

            model.Series.Add(heatMapSeries);

            Directory.CreateDirectory("out");
            // Imagemagick's convert can be used to convert .svg sequence to mp4
            // convert -delay 2 *.svg visualization.mp4
            using (var stream = File.Create($"out\\viz_{steps:0000}.svg"))
            {
                var exporter = new SvgExporter { Width = 800, Height = 800 };
                exporter.Export(model, stream);
            }
        }

        public Tile GetStart() => Grid.SelectMany(r => r).FirstOrDefault(t => t.Type == MapTileType.Start)!;

        public bool IsValidCoordinate(Coordinate c) =>
            AllowRepeat || c.Y >= 0 && c.Y < Grid.Length && c.X >= 0 && c.X < Grid[0].Length;

        private int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public Tile GetAt(Coordinate c)
        {
            if (!AllowRepeat)
            {
                return Grid[c.Y][c.X];
            }

            var origTile = Grid[Mod(c.Y, Grid.Length)][Mod(c.X, Grid[0].Length)];
            return new Tile(origTile.Type, c);
        }

        private Tile SetAt(Coordinate c, Tile t) => Grid[c.Y][c.X] = t;
    }

    public class Tile : IEquatable<Tile>
    {
        public Coordinate Coord { get; }
        public MapTileType Type { get; }

        public Tile(char t, int x, int y)
        {
            Coord = new Coordinate(x, y);
            Type = t.ParseEnum<MapTileType>();
        }

        public Tile(MapTileType t, Coordinate c)
        {
            Coord = c;
            Type = t;
        }

        public List<Tile> GetNeighbors(Map map)
        {
            var offsets = new List<Coordinate>
            {
                new(1, 0),
                new(0, 1),
                new(-1, 0),
                new(0, -1),
            };

            return offsets.Select(o => Coord + o)
                .Where(map.IsValidCoordinate)
                .Select(map.GetAt)
                .Where(t => t.Type == MapTileType.Plot)
                .ToList();
        }

        public bool Equals(Tile? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Coord.Equals(other.Coord);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tile)obj);
        }

        public override int GetHashCode()
        {
            return Coord.GetHashCode();
        }
    }

    public enum MapTileType
    {
        Rock = '#',
        Plot = '.',
        Start = 'S',
    }

    public record Coordinate(int X, int Y)
    {
        public static Coordinate operator +(Coordinate a, Coordinate b)
            => new(a.X + b.X, a.Y + b.Y);
    }
}