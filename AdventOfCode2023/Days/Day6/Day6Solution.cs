using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day6;

public class Day6Solution : Solution
{
    public Day6Solution()
    {
    }

    public Day6Solution(string input) : base(input)
    {
    }

    public override int Day => 6;

    private List<Race> Parse(bool correctBadKerning)
    {
        var lines = ReadInputLines();
        var times = lines[0].GetNumbers<long>();
        var distances = lines[1].GetNumbers<long>();

        if (correctBadKerning)
        {
            times = new List<long>() { long.Parse(string.Join("", times.Select(t => t.ToString()).ToList())) };
            distances = new List<long>() { long.Parse(string.Join("", distances.Select(t => t.ToString()).ToList())) };
        }

        return times.Zip(distances, (t, d) => new Race(t, d)).ToList();
    }

    public override string Solve(SolutionPart part)
    {
        // Let T : total time
        // Then T = h + t, where h is the time holding the button and t is the travel time of the boat
        // Then the total travelled distance is d = h * t
        //
        // But then for a given distance k, h*t=h*(T-h)=k.
        // This gives a quadratic equation -h^2 +hT -k = 0
        // And so we determine h = (T (+/-) sqrt(T^2 - 4 * k))/2
        //
        // Which gives us the time the button was held down to achieve the record distance

        return part switch
        {
            SolutionPart.PartA => Parse(false).Select(r => r.GetWinningCount()).Aggregate(1, (m, c) => m * c).ToString(),
            SolutionPart.PartB => Parse(true).Select(r => r.GetWinningCount()).Aggregate(1, (m, c) => m * c).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private class Race
    {
        public Race(long time, long distance)
        {
            Time = time;
            RecordDistance = distance;
        }

        public long Time { get; }
        public long RecordDistance { get; }

        public int GetWinningCount()
        {
            // See description above
            var sqrt = Math.Sqrt(Time * Time - 4 * RecordDistance);
            var h1 = (Time + sqrt) / 2;
            var h2 = (Time - sqrt) / 2;

            var higher = h1 % 1 == 0 ? (int)h1 - 1 : (int)Math.Floor(h1);
            var lower = h2 % 1 == 0 ? (int)h2 + 1 : (int)Math.Ceiling(h2);

            return higher - lower + 1;
        }
    }
}