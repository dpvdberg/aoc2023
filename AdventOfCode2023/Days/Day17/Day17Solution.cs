using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;
using Priority_Queue;

namespace AdventOfCode2023.Days.Day17;

public class Day17Solution : Solution
{
    public override int Day => 17;

    private class Node : IEquatable<Node>
    {
        public int Heat { get; }
        public int Y { get; }
        public int X { get; }

        public Node(int heat, int x, int y)
        {
            Heat = heat;
            Y = y;
            X = x;
        }

        public int ManhattanDistanceTo(Node other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

        public List<(Node, Movement.Direction)> Neighbors(Node[][] field, Movement.Direction d)
        {
            var l = new List<(Node, Movement.Direction)>();
            foreach (var direction in Movement.GetTurnedDirections(d))
            {
                var node = direction switch
                {
                    Movement.Direction.North => field.GetAtOrNull(Y - 1, X),
                    Movement.Direction.East => field.GetAtOrNull(Y, X + 1),
                    Movement.Direction.South => field.GetAtOrNull(Y + 1, X),
                    Movement.Direction.West => field.GetAtOrNull(Y, X - 1),
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (node != null)
                {
                    l.Add((node, direction));
                }
            }

            return l;
        }

        public bool Equals(Node? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Y == other.Y && X == other.X;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Y, X);
        }
    }

    public class Movement
    {
        public enum Direction
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        public static List<Direction> GetTurnedDirections(Direction currentDirection)
            => new()
            {
                currentDirection,
                // Round robin previous enum value
                (Direction)((((int)currentDirection - 1) % 4 + 4) % 4),
                // Round robin next enum value
                (Direction)((((int)currentDirection + 1) % 4 + 4) % 4),
            };
    }

    private class SearchState : IEquatable<SearchState>
    {
        public SearchState(Node node, Movement.Direction direction, int consecutiveDirections, int score)
        {
            Node = node;
            Direction = direction;
            ConsecutiveDirections = consecutiveDirections;
            Score = score;
        }

        public Node Node { get; }
        public Movement.Direction Direction { get; }
        public int ConsecutiveDirections { get; }
        public int Score { get; }

        public int FScore(Node destination) => Score + Heuristic(destination);
        private int Heuristic(Node destination) => Node.ManhattanDistanceTo(destination);

        public override int GetHashCode()
        {
            return HashCode.Combine(Node, (int)Direction, ConsecutiveDirections);
        }

        public bool Equals(SearchState? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Node.Equals(other.Node) && Direction == other.Direction &&
                   ConsecutiveDirections == other.ConsecutiveDirections;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SearchState)obj);
        }
    }

    /**
     * A* search, using manhattan distance to destination as a heuristic.
     * 
     * Returns total heat loss over minimum path from start to destination.
     */
    private int AStar(Node[][] field, Node start, Node destination, int minConsecutive, int maxConsecutive)
    {
        var queue = new SimplePriorityQueue<SearchState>();
        var scoreCache = new DictionaryWithDefault<SearchState, int>(int.MaxValue);

        queue.Enqueue(new SearchState(start, Movement.Direction.East, 0, 0), 0);
        queue.Enqueue(new SearchState(start, Movement.Direction.South, 0, 0), 0);

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();

            if (state.Node.Equals(destination))
            {
                return state.Score;
            }

            foreach (var (node, direction) in state.Node.Neighbors(field, state.Direction))
            {
                var straight = direction == state.Direction;
                var newConsecutive = straight ? state.ConsecutiveDirections + 1 : 1;
                if (newConsecutive > maxConsecutive)
                {
                    // Illegal state, would move straight consecutively too many times.
                    continue;
                }

                if (!straight && state.ConsecutiveDirections < minConsecutive)
                {
                    // Illegal state, would turn too fast.
                    continue;
                }

                var newScore = state.Score + node.Heat;
                var newState = new SearchState(node, direction, newConsecutive, newScore);

                if (newScore < scoreCache[newState])
                {
                    scoreCache[newState] = newScore;
                    if (!queue.Contains(newState))
                    {
                        queue.Enqueue(newState, newState.FScore(destination));
                    }
                }
            }
        }

        throw new ArgumentException("No path from from start to destination.");
    }

    private Node[][] Parse()
        => ReadInputLines().Select((l, y) =>
            l.ToCharArray().Select((c, x) => new Node(c - '0', x, y)).ToArray()
        ).ToArray();


    public override string Solve(SolutionPart part)
    {
        var field = Parse();

        var start = field[0][0];
        var end = field[^1][^1];

        return part switch
        {
            SolutionPart.PartA => AStar(field, start, end, 0, 3).ToString(),
            SolutionPart.PartB => AStar(field, start, end, 4, 10).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }
}