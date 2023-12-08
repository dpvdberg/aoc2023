using System.Text.RegularExpressions;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day5;

public class Day5Solution : Solution
{
    public Day5Solution()
    {
    }

    public Day5Solution(string input) : base(input)
    {
    }

    public override int Day => 5;

    private Farm Parse(bool individualSeeds)
    {
        var lines = ReadInputLines();

        // Get seed line and pop from list
        var seedsLine = lines[0];
        lines.RemoveAt(0);

        var seedSpecNumbers = seedsLine.Split(':')[1].Trim().Split(' ').Select(long.Parse).ToList();
        var seedRanges = individualSeeds
            ? seedSpecNumbers.Select(s => new Range(s, s)).ToList()
            : seedSpecNumbers.Zip(seedSpecNumbers.Skip(1), (a, b) => new Range(a, a + b - 1)).Where((r,i) => i % 2 == 0).ToList();

        var regex = new Regex(@"(\w+)-to-(\w+) map:\s+((?:^\d.*\s?)+)", RegexOptions.Multiline);
        var matches = regex.Matches(ReadInput());

        var mappings = matches.ToList().Select(match =>
            new Mapping(match.Groups[3].Value.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList()
            )
        ).ToList();

        return new Farm(seedRanges, mappings);
    }

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse(true).GetDestinations().MinBy(r => r.Start)!.Start.ToString(),
            SolutionPart.PartB => Parse(false).GetDestinations().MinBy(r => r.Start)!.Start.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private class Range
    {
        public Range(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long Start { get; }
        public long End { get; }

        public bool Overlap(Range other)
        {
            return this.Start <= other.End && other.Start <= this.End;
        }
    }

    private class Farm
    {
        public List<Range> Seeds { get; }

        public List<Mapping> Mappings { get; }

        public Farm(List<Range> seeds, List<Mapping> mappings)
        {
            Seeds = seeds;
            Mappings = mappings;
        }

        public List<Range> GetDestinations()
        {
            var s = new List<Range>(Seeds);

            // For all map collections
            foreach (var mapping in Mappings)
            {
                var newSeeds = new List<Range>();
                // Loop over seed ranges
                foreach (var range in s)
                {
                    bool mapped = false;
                    // For each map range in the map collection
                    foreach (var rangeMap in mapping.RangeMaps)
                    {
                        // Perform the mapping and store accordingly
                        mapped = rangeMap.Map(range, out var newRanges);
                        if (mapped)
                        {
                            newSeeds.AddRange(newRanges);
                            break;
                        }
                    }

                    if (!mapped)
                    {
                        // Keep original of no mapping was performed
                        newSeeds.Add(range);
                    }
                }

                s = newSeeds;
            }

            return s;
        }
    }

    private class Mapping
    {
        public List<RangeMap> RangeMaps { get; } = new();

        public Mapping(List<string> mapLines)
        {
            var lineRegex = new Regex(@"(\d+) (\d+) (\d+)");
            foreach (var line in mapLines)
            {
                var lineMatch = lineRegex.Match(line);
                var destination = long.Parse(lineMatch.Groups[1].Value);
                var source = long.Parse(lineMatch.Groups[2].Value);
                var length = long.Parse(lineMatch.Groups[3].Value);

                RangeMaps.Add(new RangeMap(source, destination, length));
            }
        }
    }

    private class RangeMap
    {
        public long Source { get; }
        public long Destination { get; }
        public long Length { get; }

        public Range SourceRange => new Range(Source, Source + Length - 1);

        public RangeMap(long source, long destination, long length)
        {
            Source = source;
            Destination = destination;
            Length = length;
        }

        public bool Map(Range range, out List<Range> ranges)
        {
            // We have three cases

            // 1. Range is contained by mapping range (we get 1 mapped range)
            // 2. Range partially overlaps with mapping range (we get 2 ranges)
            // 3. No overlap

            if (!SourceRange.Overlap(range))
            {
                // Case 3: no overlap
                ranges = new List<Range> { range };
                return false;
            }

            var shift = Destination - Source;

            if (SourceRange.Start <= range.Start && SourceRange.End >= range.End)
            {
                // case 1: fully contained
                // shift the entire range
                ranges = new List<Range>
                {
                    new(range.Start + shift, range.End + shift)
                };
                return true;
            }

            // case 3: partial overlap
            if (SourceRange.Start > range.Start)
            {
                // Overlap on the 'left'
                ranges =  new List<Range>
                {
                    new(range.Start, SourceRange.Start - 1), // The unmapped part
                    new(SourceRange.Start + shift, range.End + shift), // The mapped part
                };
                return true;
            }
            else
            {
                // Overlap on the 'right'
                ranges =  new List<Range>
                {
                    new(SourceRange.End + 1, range.End), // The unmapped part
                    new(range.Start + shift, SourceRange.End + shift), // The mapped part
                };
                return true;
            }
        }
    }
}