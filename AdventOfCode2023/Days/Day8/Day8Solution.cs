using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day8;

public class Day8Solution : Solution
{
    public override int Day => 8;

    public Day8Solution()
    {
    }

    public Day8Solution(string input) : base(input)
    {
    }

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse().CountSteps("ZZZ").ToString(),
            SolutionPart.PartB => Parse().CountStepsLCM("A", "Z").ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private Map Parse()
    {
        var pattern = ReadInputLines()[0];
        var directions = pattern.ToCharArray().Select(c => c.ParseEnum<Direction>()).ToList();

        var regex = new Regex(@"(\w+)\s=\s\((\w+),\s*(\w+)\)", RegexOptions.Multiline);
        var matches = regex.Matches(ReadInput());

        // First create
        var nodeDict = new Dictionary<string, Node>();
        foreach (Match m in matches)
        {
            var location = m.Groups[1].ToString();
            nodeDict[location] = new Node(location);
        }

        // Then link nodes
        foreach (Match m in matches)
        {
            var location = m.Groups[1].ToString();
            var left = m.Groups[2].ToString();
            var right = m.Groups[3].ToString();
            nodeDict[location].Left = nodeDict[left];
            nodeDict[location].Right = nodeDict[right];
        }

        return new Map(directions, nodeDict);
    }

    private enum Direction
    {
        Left = 'L',
        Right = 'R',
    }

    private class Map
    {
        public Map(List<Direction> directions, Dictionary<string, Node> nodeDictionary)
        {
            Directions = directions;
            NodeDictionary = nodeDictionary;
        }

        public List<Direction> Directions { get; }
        public Dictionary<string, Node> NodeDictionary { get; }

        public long CountSteps(string destination, string start = "AAA")
        {
            return CountSteps(destination, NodeDictionary[start]);
        }

        public long CountSteps(string suffix, Node start)
        {
            long steps = 0;
            Node current = start;

            while (!current.Location.EndsWith(suffix))
            {
                var direction = Directions[(int)(steps % Directions.Count)];
                current = current.GetNext(direction);

                steps++;
            }

            return steps;
        }

        public long CountStepsLCM(string startSuffix, string endSuffix)
        {
            // Basically we need to find LCM of all cycles

            var startNodes = NodeDictionary.Values.Where(n => n.Location.EndsWith(startSuffix));
            var cycleTimes = startNodes.Select(n => CountSteps(endSuffix, n));

            return cycleTimes.LeastCommonMultiple();
        }
    }

    private class Node
    {
        public Node(string location)
        {
            Location = location;
        }

        public string Location { get; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node GetNext(Direction d)
            => d switch
            {
                Direction.Left => Left,
                Direction.Right => Right,
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
    }
}